using Genshin_Checker.GUI.UserInterface.Setting.Category;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Genshin_Checker.Core.General.Convert;
using static Genshin_Checker.Core.General.Convert.Rarity;
using static Genshin_Checker.Core.General.Convert.Element;
using Genshin_Checker.Core.HoYoLab;

namespace Genshin_Checker.ViewModel.CharacterCalculator
{
    /*
    public class CharacterCalculator : ViewModelBase
    {
        public CharacterCalculator()
        {
            Items1 = CreateData();
            Items2 = CreateData();
            Items3 = CreateData();
            Items4 = CreateData();

            foreach (var model in Items1)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(CharacterInfo.IsSelected))
                        OnPropertyChanged(nameof(IsAllItemsSelected));
                };
            }

            Files = new List<string>();

            for (int i = 0; i < 1000; i++)
            {
                Files.Add(Path.GetRandomFileName());
            }
        }

        public bool? IsAllItemsSelected
        {
            get
            {
                var selected = Items1.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, Items1);
                    OnPropertyChanged();
                }
            }
        }

        private static void SelectAll(bool select, IEnumerable<CharacterInfo> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        private static async ObservableCollection<CharacterInfo> UpdateData(Account account)
        {

            var charas = await account.Characters.GetData();
            await account.CharacterDetail.UpdateGameData(true);
            var detail = account.CharacterDetail.CachedCharacters();
            var list = new ObservableCollection<CharacterInfo>();
            //ここにcharasからCharacterInfoに変換する処理を書く
            detail.ForEach(chara =>
            {
                var skills = chara.skills.FindAll(a => Core.General.Convert.Character.GetSkillGrowthable(a.skill_id, chara.baseInfo.id) && a.skill_type == 1);
                if (skills.Count != 3)
                {
                    Log.Warn($"スキルが3つでないキャラクターがいるためスキップします。 {chara.baseInfo.id} : {chara.baseInfo.name}");
                    return;
                }
                list.Add(new CharacterInfo
                {
                    Id = chara.baseInfo.id,
                    Name = chara.baseInfo.name,
                    Rarity = GetRarityType(chara.baseInfo.rarity),
                    Element = GetElementEnum(chara.baseInfo.element),
                    Level = chara.baseInfo.level,
                    Constellation = chara.baseInfo.actived_constellation_num,
                    Fetter = chara.baseInfo.fetter,
                    WeaponType = Character.GetWeaponTypeName(chara.baseInfo.weapon_type),
                    CurrentNormalAttackLevel = skills[0].level,
                    CurrentElementalSkillLevel = skills[1].level,
                    CurrentElementalBurstLevel = skills[2].level,
                    TargetNormalAttackLevel = chara.normalAttackLevel,
                    TargetElementalSkillLevel = chara.elementalSkillLevel,
                    TargetElementalBurstLevel = chara.elementalBurstLevel
                });
            });



            return
        {
                new SelectableViewModel
                {
                    Code = 'M',
                    Name = "Material Design",
                    Description = "Material Design in XAML Toolkit"
                },
            new SelectableViewModel
            {
                Code = 'D',
                Name = "Dragablz",
                Description = "Dragablz Tab Control",
                Food = "Fries"
            },
            new SelectableViewModel
            {
                Code = 'P',
                Name = "Predator",
                Description = "If it bleeds, we can kill it"
            }
        };
        }

        public ObservableCollection<CharacterInfo> Items1 { get; }

        public IEnumerable<string> Foods => new[] { "Burger", "Fries", "Shake", "Lettuce" };

        public IList<string> Files { get; }

        public IEnumerable<DataGridSelectionUnit> SelectionUnits => new[] { DataGridSelectionUnit.FullRow, DataGridSelectionUnit.Cell, DataGridSelectionUnit.CellOrRowHeader };
    }
    public class CharacterInfo : ViewModelBase
    {
        private bool _isSelected;
        private int _id;
        private RarityType _rarity;
        private ElementType _element;
        private string? _name;
        private string? _weaponType;
        private int _constellation;
        private int _fetter;
        private int _level;
        private int _currentNormalAttackLevel;
        private int _currentElementalSkillLevel;
        private int _currentElementalBurstLevel;

        private int _targetNormalAttackLevel;
        private int _targetElementalSkillLevel;
        private int _targetElementalBurstLevel;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public RarityType Rarity
        {
            get => _rarity;
            set => SetProperty(ref _rarity, value);
        }
        public ElementType Element
        {
            get => _element;
            set => SetProperty(ref _element, value);
        }

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? WeaponType
        {
            get => _weaponType;
            set => SetProperty(ref _weaponType, value);
        }

        public int Constellation
        {
            get => _constellation;
            set => SetProperty(ref _constellation, value);
        }

        public int Fetter
        {
            get => _fetter;
            set => SetProperty(ref _fetter, value);
        }

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }
        public int TargetNormalAttackLevel
        {
            get => _targetNormalAttackLevel;
            set => SetProperty(ref _targetNormalAttackLevel, value);
        }
        public int TargetElementalSkillLevel
        {
            get => _targetElementalSkillLevel;
            set => SetProperty(ref _targetElementalSkillLevel, value);
        }
        public int TargetElementalBurstLevel
        {
            get => _targetElementalBurstLevel;
            set => SetProperty(ref _targetElementalBurstLevel, value);
        }
        public int CurrentNormalAttackLevel
        {
            get => _currentNormalAttackLevel;
            set => SetProperty(ref _currentNormalAttackLevel, value);
        }
        public int CurrentElementalSkillLevel
        {
            get => _currentElementalSkillLevel;
            set => SetProperty(ref _currentElementalSkillLevel, value);
        }
        public int CurrentElementalBurstLevel
        {
            get => _currentElementalBurstLevel;
            set => SetProperty(ref _currentElementalBurstLevel, value);
        }
    }
    */
}
