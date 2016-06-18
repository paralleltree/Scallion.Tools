using System;
using System.Collections.Generic;
using System.Linq;

using NDesk.Options;
using Scallion.Tools.Essentials;
using Scallion.DomainModels;
using Scallion.DomainModels.Components;

namespace Scallion.Tools.VmdCut
{
    class Program : Runner
    {
        public override string Description
        {
            get { return "モーションファイルの指定の範囲を切り出し出力します。"; }
        }

        public override string ArgumentFormat
        {
            get { return "[INPUT] [OPTIONS]... -o [OUTPUT]"; }
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
                { "?|h|help", "ヘルプを表示し、終了します。",  _ => Parameter.ShowHelp = _ != null },
                { "f|force", "出力ファイルを警告なく上書きします。", _ => Parameter.ForceOverwrite = _ != null },
                { "o=|out=", "出力先を指定します。", (string s) => Parameter.OutputFile = s },
                { "start=", "開始フレームのインデックスを指定します。指定がない場合は0が指定されます。", (int i) => Parameter.StartFrameIndex = i },
                { "end=", "終了フレームのインデックスを指定します。指定がない場合は末尾まで出力します。", (int i) => Parameter.EndFrameIndex = i }
            };

            var file = opt.Parse(args);

            if (Parameter.ShowHelp || file.Count != 1)
            {
                if (args.Length > 0 && !Parameter.ShowHelp)
                {
                    throw new MissingArgumentException("実行には入力ファイルの指定が必要です。");
                }
                PrintHelp(opt);
                Environment.Exit(Parameter.ShowHelp ? 0 : 1);
            }

            if (Parameter.OutputFile == null)
                throw new MissingArgumentException("出力先が指定されていません。");

            Parameter.InputFile = file.Single();
        }

        public override void Run()
        {
            var input = new Motion().Load(Parameter.InputFile);

            var keyframes = input.Bones.SelectMany(p => p.KeyFrames).Cast<KeyFrame>()
                .Concat(input.Morphs.SelectMany(p => p.KeyFrames))
                .Concat(input.Camera.KeyFrames)
                .Concat(input.Light.KeyFrames)
                .Concat(input.SelfShadow.KeyFrames)
                .Concat(input.VisibilityKeyFrames);

            int end = Parameter.EndFrameIndex ?? keyframes.Max(p => p.KeyFrameIndex);

            foreach (var bone in input.Bones)
            {
                bone.KeyFrames = bone.KeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();
            }

            foreach (var ik in input.IKBones)
            {
                if (ik.IKStateKeyFrames.Where(p => p.KeyFrameIndex == Parameter.StartFrameIndex).Count() == 0)
                {
                    ik.IKStateKeyFrames.Add(new IKStateKeyFrame()
                    {
                        KeyFrameIndex = Parameter.StartFrameIndex,
                        IsIKEnabled = ik.IKStateKeyFrames
                            .Where(p => p.KeyFrameIndex < Parameter.StartFrameIndex)
                            .OrderByDescending(p => p.KeyFrameIndex)
                            .First().IsIKEnabled
                    });
                }
                ik.IKStateKeyFrames = ik.IKStateKeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();
            }

            foreach (var morph in input.Morphs)
                morph.KeyFrames = morph.KeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();

            input.Camera.KeyFrames = input.Camera.KeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();
            input.Light.KeyFrames = input.Light.KeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();
            input.SelfShadow.KeyFrames = input.SelfShadow.KeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();

            if (input.VisibilityKeyFrames.Where(p => p.KeyFrameIndex == Parameter.StartFrameIndex).Count() == 0)
            {
                input.VisibilityKeyFrames.Add(new VisibilityKeyFrame()
                {
                    KeyFrameIndex = Parameter.StartFrameIndex,
                    IsVisible = input.VisibilityKeyFrames
                        .Where(p => p.KeyFrameIndex <= Parameter.StartFrameIndex)
                        .OrderByDescending(p => p.KeyFrameIndex)
                        .First().IsVisible
                });
            }
            input.VisibilityKeyFrames = input.VisibilityKeyFrames.FilterKeyFrame(Parameter.StartFrameIndex, end).ToList();

            if (!Parameter.ForceOverwrite && !Parameter.OutputFile.ConfirmOverwrite()) return;
            input.Save(Parameter.OutputFile);
        }
    }

    class ExecutionParameter
    {
        public bool ShowHelp { get; set; }
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public bool ForceOverwrite { get; set; }
        public int StartFrameIndex { get; set; } = 0;
        public int? EndFrameIndex { get; set; }
    }

    static class Extensions
    {
        public static IEnumerable<T> FilterKeyFrame<T>(this IEnumerable<T> src, int start, int end) where T : KeyFrame
        {
            return src.Where(p => p.IsContained(start, end))
                .Select(p =>
                {
                    p.KeyFrameIndex -= start;
                    return p;
                });
        }

        public static bool IsContained<T>(this T src, int start, int end) where T : KeyFrame
        {
            return src.KeyFrameIndex >= start && src.KeyFrameIndex <= end;
        }
    }
}
