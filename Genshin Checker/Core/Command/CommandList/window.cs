using Genshin_Checker.Core.Game;
using Genshin_Checker.Core.General;
using Genshin_Checker.Window.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Checker.Core.Command.CommandList
{
    public class DebugWindow : Command
    {
        public override string Name => "window";
        public override string Description => "デバッグウィンドウを表示します。";

        public override async Task Execute(params string[] parameters)
        {
            if (parameters.Length == 1)
            {
                Console("api-checker -> APIのデバッグメニューを開きます。");
                return;
            }

            switch (parameters[1].ToLower())
            {
                case "api-checker":
                    Console("現在ウィンドウを表示しています。");
                    APIChecker window = new APIChecker();
                    window.ShowDialog();
                    await Task.Delay(1);
                    break;
                default:
                    Console($"{parameters[1]} は不明なパラメータです。");
                    break;
            }
            return;
        }
    }
}
