using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using System.Numerics;
using NDesk.Options;
using Scallion.Tools.Essentials;
using Scallion.DomainModels;

namespace Scallion.Tools.VmdScale
{
    class Program : Runner
    {
        public override string Description
        {
            get { return "モーションファイルをスケーリングし出力します。\nデフォルトではファイル名に倍率を付加して出力します。"; }
        }

        public override string ArgumentFormat
        {
            get { return "[OPTION]... [INPUT] [OUTPUT]"; }
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
                { "?|h|help", "ヘルプを表示し、終了します。", _ =>  Parameter.ShowHelp = _ != null },
                { "s=|scale=", "スケーリングの倍率を指定します。", (float f) => Parameter.ScaleFactor = f }
            };

            var files = opt.Parse(args);

            if (Parameter.ShowHelp || files.Count != 2)
            {
                if (args.Length > 0 && !Parameter.ShowHelp)
                {
                    throw new MissingArgumentException("実行には2つのファイルの指定が必要です。");
                }
                PrintHelp(opt);
                Environment.Exit(Parameter.ShowHelp ? 0 : 1);
            }

            Parameter.InputFile = files[0];
            Parameter.OutputFile = files[1];
        }

        public override void Run()
        {
            var input = new Motion().Load(Parameter.InputFile);

            foreach (var bone in input.Bones)
                foreach (var keyframe in bone.KeyFrames)
                    keyframe.Position = keyframe.Position.ScaleVector3(Parameter.ScaleFactor);

            foreach (var keyframe in input.Camera.KeyFrames)
                keyframe.Position = keyframe.Position.ScaleVector3(Parameter.ScaleFactor);

            if (File.Exists(Parameter.OutputFile))
            {
                while (true)
                {
                    Console.Write("出力ファイルを上書きしますか？ (y/N) > ");
                    string res = Console.ReadLine();
                    if (res == "" || Regex.IsMatch(res, "^(y|n)$", RegexOptions.IgnoreCase))
                    {
                        if (res.ToLower() == "y") break;
                        else return;
                    }
                }
            }

            input.Save(Parameter.OutputFile);
        }
    }

    class ExecutionParameter
    {
        public bool ShowHelp { get; set; }
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
    }

    static class Extensions
    {
        public static Vector3 ScaleVector3(this Vector3 src, float scale)
        {
            return new Vector3(src.X * scale, src.Y * scale, src.Z * scale);
        }
    }
}
