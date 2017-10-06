
using System;
using System.Collections.Generic;
using System.Linq; // コレクションにLINQをはやすにはこれ
using Xunit;


namespace CsharpPractice
{
    public class LinqTest
    {
        [Fact]
        public void Linqスタイル()
        {

            var list = new List<String> { "11", "2", "333" };

            // listから要素 x を取り出し、 xのLength が1より大きいものについて、intにする。
            var ints = from x in list where x.Length > 1 select int.Parse(x);

            Assert.Equal(344, ints.Sum());

        }

        [Fact]
        public void ラムダ式スタイル()
        { 

            var list = new List<String> { "11", "2", "333" };

            var ints2 = list.Where(x => x.Length > 1).Select(int.Parse);

            Assert.Equal(344, ints2.Sum());
        }
    }
}
