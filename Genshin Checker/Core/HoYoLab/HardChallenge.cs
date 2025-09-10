using Genshin_Checker.Core.General;
using Genshin_Checker.Core.General.Convert;
using Genshin_Checker.Model.Misaki_chan.info;
using Genshin_Checker.Model.UserData;
using Genshin_Checker.Model.UserData.HardChallenge.v1;
using Genshin_Checker.Resource.Languages;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Localize = Genshin_Checker.Resource.Languages.Localize;

namespace Genshin_Checker.Core.HoYoLab
{
    public class HardChallenge : Base
    {

        public HardChallenge(Account account) : base(account, 5000)
        {
            ServerUpdate.Elapsed += Timeout_Tick;
        }
        private Model.HoYoLab.HardChallenge.Data? hardChallenge= null;
        private string REG_PATH { get => $"UserData\\{account.UID}\\HardChallenge"; }
        public V1? Current { get; private set; }
        bool IsFirstCheck = false;
        internal async void Timeout_Tick(object? sender, EventArgs e)
        {
            ServerUpdate.Stop();
            Log.Debug("幽境を取得");
            await ScheduleReload();
            ServerUpdate.Interval = (account.LatestActiveSession > DateTime.UtcNow.AddHours(-2) || account.LatestActivity == Game.ProcessTime.ProcessState.Foreground) ? 600000 : 3600000 * 3;
            ServerUpdate.Start();
        }
        private async Task ScheduleReload()
        {
            try
            {
                var Data = await GetData();
                await SaveDatabase(Data);

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public async Task<V1> Load(string id)
        {
            var path = Registry.GetValue(REG_PATH, $"{id}", true) ?? throw new IOException(Localize.Error_SpiralAbyssFile_RegistryNotFound);
            var data = await AppData.LoadUserData(path);
            if (string.IsNullOrEmpty(data)) throw new InvalidDataException("Data is empty.");
            var ver = JsonChecker<Model.UserData.DatabaseRoot>.Check(data ?? "{}");
            V1? v1 = (ver?.Version) switch
            {
                null => throw new InvalidDataException(Localize.Error_SpiralAbyssFile_InvalidFileVersion),
                1 => JsonChecker<V1>.Check(data ?? ""),
                _ => throw new InvalidDataException(string.Format(Localize.Error_SpiralAbyssFile_UnknownFileVersion, ver.Version)),
            } ?? throw new InvalidDataException(Localize.Error_SpiralAbyssFile_FailedConvert);
            if (ver?.Version < 1)
            {
                Log.Info("バージョンアップグレードします。");
                await Save(v1);
            }
            if (v1.UID != account.UID) throw new InvalidDataException(string.Format(Localize.Error_SpiralAbyssFile_DoesNotMatchUID, v1.UID, account.UID));
            return v1;
        }
        public async Task<Model.HoYoLab.HardChallenge.Data> GetData()
        {
            var data = await account.Endpoint.GetHardChallenge(true);
            hardChallenge = data;
            return data;
        }

        private async Task Save(V1 v1)
        {
            string? path = Registry.GetValue(REG_PATH, $"{v1.Data.schedule.schedule_id}", true);
            if (path == null)
            {
                path = AppData.GetRandomPath();
                Registry.SetValue(REG_PATH, $"{v1.Data.schedule.schedule_id}", path, true);

            }
            await AppData.SaveUserData(path, JsonConvert.SerializeObject(v1));
        }
        private async Task<int> SaveDatabase(Model.HoYoLab.HardChallenge.Data raw)
        {
            Log.Info("幽境のデータを保存します。");
            int CountOfNewData = 0;
            foreach (var index in raw.data)
            {
                var userdata = new V1();
                #region ユーザーデータベースから過去の情報読み出し
                Log.Debug($"幽境ID {index.schedule.schedule_id}");
                var path = Registry.GetValue(REG_PATH, $"{index.schedule.schedule_id}", true);
                if (path != null && AppData.IsExistFile(path))
                {
                    Log.Debug($"→データが見つかりました。");
                    userdata = await Load(index.schedule.schedule_id);
                    if (userdata.UID != account.UID)
                        throw new InvalidDataException(
                            string.Format(Localize.Error_SpiralAbyssFile_DoesNotMatchUID, userdata.UID, account.UID));
                }
                #endregion
                #region 取得したデータが重複しているかチェック
                userdata.UID = account.UID;
                userdata.UpdateUTC = DateTime.UtcNow;
                userdata.Version = 1; //v1
                var starttime = DateTime.MaxValue;
                var endtime = DateTime.MinValue;
                starttime = new DateTime(1970,1,1,0,0,0).AddSeconds(int.Parse(index.schedule.start_time));
                endtime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(int.Parse(index.schedule.end_time));
                Log.Debug($"取得したデータは {starttime} - {endtime} です。");
                if (Current == null || Current.Data.schedule.schedule_id <= userdata.Data.schedule.schedule_id) Current = userdata;
                bool IsNextMove = false;

                var single = userdata.Data.single.Find(a =>
                {
                    return (a.best.second == index.single.best?.second &&
                    a.best.difficulty == index.single.best?.difficulty);
                });

                var multiplay = userdata.Data.mp.Find(a =>
                {
                    return (a.best.second == index.mp.best?.second &&
                    a.best.difficulty == index.mp.best?.difficulty);
                });
                #endregion
                if (IsNextMove && IsFirstCheck)
                {
                    Log.Debug($"→データに更新が無さそうなのでスキップします。");
                    continue; //データに更新が無い場合はスキップ
                }
                else
                {
                    IsFirstCheck = true;
                }
                bool IsSingleNewData = single == null;
                bool IsMultiplayNewData = multiplay == null;
                single ??= new();
                multiplay ??= new();
                #region 内容のコピー

                //ここはスケジュール情報
                if (int.TryParse(index.schedule.start_time, out var start)) userdata.Data.schedule.start_time= Time.GetUTCFromUnixTime(start);
                if (int.TryParse(index.schedule.end_time, out var end)) userdata.Data.schedule.end_time = Time.GetUTCFromUnixTime(end);
                userdata.Data.schedule.schedule_id = int.Parse(index.schedule.schedule_id);

                //ここは現時点でのデータ
                userdata.Data.blings = index.blings
                    .Select(a => new Bling() { avater_id = a.avatar_id, is_plus = a.is_plus })
                    .ToList();
                if (index.single.has_data && index.single.best != null)
                {
                    single.created_at = DateTime.UtcNow;
                    single.best.icon = index.single.best.icon;
                    single.best.difficulty = index.single.best.difficulty;
                    single.best.second = index.single.best.second;
                    single.challenge = index.single.challenge.Select(a => new Challenge()
                    {
                        best_avatar = a.best_avatar.Select(b => new BestAvatar() { avatar_id = b.avatar_id, damages = int.Parse(b.dps), type = b.type }).ToList(),
                        teams = a.teams.Select(b => new Team() { avatar_id = b.avatar_id, level = b.level, rank = b.rank }).ToList(),
                        name = a.name,
                        monster = new()
                        {
                            name = a.monster.name,
                            desc = a.monster.desc,
                            icon = a.monster.icon,
                            level = a.monster.level,
                            monster_id = a.monster.monster_id,
                            tags = a.monster.tags.Select(b => new Tag() { desc = b.desc, type = b.type }).ToList()
                        },
                        second = a.second
                    }).ToList(); ;
                    Log.Debug($"シングル: 難易度{single.best.difficulty} {single.best.second}秒");
                    if (IsSingleNewData)
                    {
                        Log.Debug($"→今回取得したデータは新しい為追加します。");
                        userdata.Data.single.Add(single);
                        CountOfNewData++;
                    }
                }

                if (index.mp.has_data && index.mp.best != null)
                {
                    multiplay.created_at = DateTime.UtcNow;
                    multiplay.best.icon = index.mp.best.icon;
                    multiplay.best.difficulty = index.mp.best.difficulty;
                    multiplay.best.second = index.mp.best.second;
                    multiplay.challenge = index.mp.challenge.Select(a => new Challenge()
                    {
                        best_avatar = a.best_avatar.Select(b => new BestAvatar() { avatar_id = b.avatar_id, damages = int.Parse(b.dps), type = b.type }).ToList(),
                        teams = a.teams.Select(b => new Team() { avatar_id = b.avatar_id, level = b.level, rank = b.rank }).ToList(),
                        name = a.name,
                        monster = new()
                        {
                            name = a.monster.name,
                            desc = a.monster.desc,
                            icon = a.monster.icon,
                            level = a.monster.level,
                            monster_id = a.monster.monster_id,
                            tags = a.monster.tags.Select(b => new Tag() { desc = b.desc, type = b.type }).ToList()
                        },
                        second = a.second
                    }).ToList();
                    Log.Debug($"マルチ: 難易度{multiplay.best.difficulty} {multiplay.best.second}秒");
                    if (IsMultiplayNewData)
                    {
                        Log.Debug($"→今回取得したデータは新しい為追加します。");
                        userdata.Data.mp.Add(multiplay);
                        CountOfNewData++;
                    }
                }


                #endregion
                await Save(userdata);
                Log.Info($"スケジュールID {userdata.Data.schedule.schedule_id} 保存しました。");
            }
            if (CountOfNewData == 0) Log.Info("今回は保存されませんでした。");
            return CountOfNewData;
        }
    }
}
