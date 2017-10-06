
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; // コレクションにLINQをはやすにはこれ
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace CsharpPractice
{


    public class TaskTest
    {
        /// <summary>
        /// 時間のかかる処理
        /// </summary>
        /// <param name="ans"></param>
        /// <returns></returns>
        public static int HeavyProcess(int ans)
        {

            Thread.Sleep(500);
            Debug.WriteLine("End Heavy Process");
            return ans;
        }

        [Fact]
        public void Taskの基本() // Taskを扱うテストは Taskを戻り値にすると、xunitが上手く扱ってくれる。
        {
            // Task.Runで、非同期で処理が開始され、結果が返ってくるのをまたない。
            var t1 = Task.Run(() => HeavyProcess(42));
            Debug.WriteLine("Kicked Heavy Process"); // こちらが先に出力される。

            // 結果を取得するには、Wait を使う。
            // ただし、結果を取得するまで自分のスレッドが停止(ブロック)するので、
            // 原則的に waitは使わない。
            t1.Wait();
            Assert.Equal(42, t1.Result);
        }

        [Fact]
        public Task タスクの継続とユニットテスト()
        {
            // 今やっているタスクの結果に、何かを処理したい場合に、
            // ↑のようは Wait & Result を使ってはいけない。
            /*
            var t1 = Task.Run(() => HeavyProcess(42));
            t1.Wait(); // ここでブロックしてしまい、自分の仕事ができなくなる。! 
            var answer = t1.Result + 42; 
            */

            // 今のタスクに継続して、何か処理をさせたい場合は Taskの ContinueWith を使って次にやることを依頼する。
            Task<int> t1 = Task.Run(() => HeavyProcess(42));
            // ContinuewWithのラムダ式は、 t1のタスクが完了した後に実行される。
            // 戻り値に t2 もタスクであり、 t1とContinuesWithの内容を1つにまとめた Taskになる。
            // タスクをつなげて、でかいタスクを作っているというイメージを持つ。
            Task<int> t2 = t1.ContinueWith(finishedTask => finishedTask.Result + 42);

            // t2は、非同期処理t1に次にやることを依頼しただけなので、相変わらず非同期処理。
            // 自スレッドはブロックしないので、↓の処理は即時で実行。
            Debug.WriteLine("Kicked T1& T2"); // こちらが先に出力される。

            // Taskの内容をxunitでアサートする場合、メソッド中でwaitせず、アサート内容をタスクに継続して、Taskを返す。
            // そうすると xunit側が waitしてアサートしてくれる。
            return t2.ContinueWith(finishedT2 => Assert.Equal(84, finishedT2.Result));
        }

        [Fact]
        public async Task async_awaitによるTaskの継続() // シグネチャに asyncを加える。
        {
            // タスクを continueWithで合成していくと、見づらくなる。
            // そのために、async, await とという特殊な文法がサポートされている。
            // ↑と同じ処理が次のように書ける。

            Task<int> t1 = Task.Run(() => HeavyProcess(42)); // 非同期処理の開始
            // t1 ContinueWithに記載したい内容を次のように書ける。
            var answer = await t1 + 42; // ↑の ContinueWith に書いた内容と同じ。

            // await Task<T> の戻り値は、そのタスク終了後の結果値 t になり、
            // さらにcontinueWith内のラムダ式の内容を続けて書くようになる。

            // そのため　↓の メインスレッドで行っていた処理は混ぜることができなくなる。
            // Debug.WriteLine("Kicked T1& T2"); // これを書くと、Taskの中で行われる処理になってしまう。

            // Assertもそのままかける。
            Assert.Equal(84, answer);

            // ブロックの最後が、ContinueWith の終わりとなり、ここまでが1つのTaskとして生成され、呼び出し元にリターン。
            // よって戻り値が Taskになる。
        }


        [Fact]
        public async Task 複数Task_直列()
        {
            var a1 = await Task.Run(() => HeavyProcess(42));
            var a2 = await Task.Run(() => HeavyProcess(42)); // これが実際に動くのは、前のタスクが終わってからになる。

            // テストの処理時間注目
            Assert.Equal(84, a1 + a2);
        }


        [Fact]
        public async Task 複数Task_並列()
        {
            // とりあえず Taskを開始しておいて、、、
            var a1 = Task.Run(() => HeavyProcess(42));
            var a2 = Task.Run(() => HeavyProcess(42));

            // あとで await すると並列にできる。処理時間注目
            Assert.Equal(84, await a1 + await a2);
            /* わかりにくければ、次のように考える。
            var res1 = await a1; 
            var res2 = await a2;
            Assert.Equal(84, res1 + res2);
            */
        }

        [Fact]
        public async Task 複数Task_ループ()
        {
            var tasks = new List<Task<int>> {
              Task.Run(() => HeavyProcess(42)),
              Task.Run(() => HeavyProcess(42)),
              Task.Run(() => HeavyProcess(42)),
            };

            // 複数のタスクは、 WhenAll で、1つのタスクにまとめることができる。
            // 並列計算もされる。結果は各タスクの結果を配列にしたもの。
            int[] res = await Task.WhenAll(tasks);

            Assert.Equal(126, res.Sum());

        }

        [Fact]
        public async Task 複数Task_ループ2()
        {
            var tasks = new List<Task<int>> {
              Task.Run(() => HeavyProcess(42)),
              Task.Run(() => HeavyProcess(42)),
              Task.Run(() => HeavyProcess(42)),
            };

            // 普通にループ中で await もできるが順次実行になるのは気をつけよう
            int res = 0;
            for(int i = 0; i < 3; i++)
            {
                res += await Task.Run(() => HeavyProcess(42));
            }
            Assert.Equal(126, res);

        }
    }
}