using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using NDesk.Options;
using Scallion.Tools.Essentials;
using Scallion.DomainModels;
using Scallion.DomainModels.Components;

namespace Scallion.Tools.VmdStat
{
    class Program : Runner
    {
        public override string Description
        {
            get { return "モーションファイルの情報を表示します。"; }
        }

        public override string ArgumentFormat
        {
            get { return "[INPUT]..."; }
        }

        ExecutionParameter Parameter = new ExecutionParameter();


        static void Main(string[] args)
        {
            Runner.Execute(() => new Program(args));
        }

        private Program(string[] args)
        {
            var opt = new OptionSet()
            {
                { "?|h|help", "ヘルプを表示し、終了します。", _ =>  Parameter.ShowHelp = _ != null }
            };

            var files = opt.Parse(args);

            if (Parameter.ShowHelp || files.Count == 0)
            {
                PrintHelp(opt);
                Environment.Exit(Parameter.ShowHelp ? 0 : 1);
            }

            Parameter.InputFiles = files;
        }

        public override void Run()
        {
            foreach (string path in Parameter.InputFiles)
            {
                var input = new Motion().Load(path);
                Console.WriteLine("{0}:", Path.GetFileName(path));
                Console.WriteLine("モデル名: {0}", input.ModelName);

                Console.WriteLine("[ボーン]");
                Console.WriteLine("ボーン数: {0}", input.Bones.Count);
                Console.WriteLine("IKボーン数: {0}", input.IKBones.Count());
                input.Bones.SelectMany(p => p.KeyFrames).ToList().PrintKeyFrameStat();

                Console.WriteLine("[モーフ]");
                Console.WriteLine("モーフ数: {0}: ", input.Morphs.Count);
                input.Morphs.SelectMany(p => p.KeyFrames).ToList().PrintKeyFrameStat();

                Console.WriteLine("[カメラ]");
                input.Camera.KeyFrames.PrintKeyFrameStat();

                Console.WriteLine("[照明]");
                input.Light.KeyFrames.PrintKeyFrameStat();

                Console.WriteLine("[セルフシャドウ]");
                input.SelfShadow.KeyFrames.PrintKeyFrameStat();

                Console.WriteLine("[表示]");
                input.VisibilityKeyFrames.PrintKeyFrameStat();

                Console.WriteLine();
            }
        }
    }

    class ExecutionParameter
    {
        public bool ShowHelp { get; set; }
        public IEnumerable<string> InputFiles { get; set; }
    }

    static class Extensions
    {
        public static void PrintKeyFrameStat<T>(this List<T> src) where T : KeyFrame
        {
            Console.WriteLine("総キーフレーム数: {0}", src.Count);
            if (src.Count > 0) Console.WriteLine("最終インデックス: {0}", src.Max(q => q.KeyFrameIndex));
        }
    }
}
