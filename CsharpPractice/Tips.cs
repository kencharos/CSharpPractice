
using System;
using System.Collections.Generic;
using System.Linq; // コレクションにLINQをはやすにはこれ
using Xunit;


namespace CsharpPractice
{
    public class Hoge
    {
        public string Name { get; set; }
        public int Age { get; set; }

    }

    public class Tips
    {

        [Fact]
        public void コンストラクタ初期化子とvarキーワード()
        {
            // コンストラクタで、{} を使うとフィールドの代入ができる。
            // var を使うと宣言の型が省略できる。
            var o1 = new Hoge { Name = "a", Age = 18 };

            // これと同じ
            Hoge o2 = new Hoge();
            o2.Name = "a";
            o2.Age = 18;

            Assert.Equal(o1.Name, o2.Name);
            Assert.Equal(o1.Age, o2.Age);

        }

        [Fact]
        public void コンストラクタ初期化子をコレクションで()
        {

            // コレクションにも使用可能。
            var strList = new List<string> { "a", "b" };
            var strList2 = new List<string>();
            strList2.Add("a");
            strList2.Add("b");

            // リストのアクセスは、配列と同じ。
            Assert.Equal(strList[1], strList2[1]);

            // Dictonaryも可能。
            var dict1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" } };
            var dict2 = new Dictionary<int, string>();
            dict2.Add(1, "A");
            dict2.Add(2, "B");

            // Dictonaryのアクセスも、配列と同じ。
            Assert.Equal(dict1[1], dict2[1]);


        }

    }
}
