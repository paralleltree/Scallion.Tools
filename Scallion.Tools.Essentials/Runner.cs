using System;
using System.Collections.Generic;
using System.Linq;

using NDesk.Options;
using static Scallion.Tools.Essentials.StaticInfo;

namespace Scallion.Tools.Essentials
{
    public interface IRunnable
    {
        void Run();
    }

    /// <summary>
    /// Represents a runner that accepts arguments given as a array of <see cref="string"/>.
    /// This class is abstract.
    /// </summary>
    public abstract class Runner : IRunnable
    {
        /// <summary>
        /// Gets the description of this runner.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the text represents a format of arguments that this runner accepts.
        /// </summary>
        public abstract string ArgumentFormat { get; }


        /// <summary>
        /// Performs its own process.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Write help text with the assembly name, version, <see cref="Description"/>, and <see cref="ArgumentFormat"/> to <see cref="Console.Out"/>.
        /// </summary>
        /// <param name="opt">An instance of <see cref="OptionSet"/> to print its options.</param>
        protected void PrintHelp(OptionSet opt)
        {
            Console.WriteLine("{0} {1}", Name, AssemblyVersion);
            Console.WriteLine(Description);
            Console.WriteLine();
            Console.WriteLine("[使い方]");
            Console.WriteLine("{0} {1}", Name, ArgumentFormat);
            if (opt.Count == 0) return;
            Console.WriteLine();
            Console.WriteLine("[オプション]");
            opt.WriteOptionDescriptions(Console.Out);
        }


        /// <summary>
        /// Execute the <see cref="IRunnable.Run"/> method in the instance
        /// with exception handler that is commonly used, after the specified initialization.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="instance"/> to execute <see cref="IRunnable.Run"/></typeparam>
        /// <param name="initializer">A function to initialize the instance of <typeparamref name="T"/>.</param>
        public static void Execute<T>(Func<T> initializer) where T : IRunnable
        {
            try
            {
                initializer().Run();
            }
            catch (OptionException ex)
            {
                Console.WriteLine("エラー: オプション {0} の指定が不正です。", ex.OptionName);
                Console.WriteLine("詳細を表示するには '{0} --help' を実行してください。", Name);
                Environment.Exit(1);
            }
            catch (MissingArgumentException ex)
            {
                Console.WriteLine("エラー: {0}", ex.Message);
                Console.WriteLine("詳細を表示するには '{0} --help' を実行してください。", Name);
                Environment.Exit(1);
            }
            catch (Exception ex) when (ex is System.IO.FileNotFoundException || ex is System.IO.DirectoryNotFoundException)
            {
                string path = System.Text.RegularExpressions.Regex.Match(ex.Message, @"'.+'").Value;
                Console.WriteLine("エラー: ファイル {0} は見つかりませんでした。", path);
                Environment.Exit(1);
            }
            catch (System.IO.EndOfStreamException)
            {
                Console.WriteLine("ファイルの読み込み中にエラーが発生しました。");
                Console.WriteLine("古いバージョンのファイルを読み込もうとした可能性があります。");
                Environment.Exit(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラー({0}): {1}", ex.GetType().ToString(), ex.Message);
                Console.WriteLine("詳細情報:");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(2);
            }
        }
    }
}
