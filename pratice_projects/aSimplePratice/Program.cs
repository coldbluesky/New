using System.Collections;
using System.Linq;

//string[,] poem = new string[4,5] {
//                                 {"春","眠","不","觉","晓"},
//                                 {"处","处","闻","啼","鸟"},
//                                 {"夜","来","风","雨","声"},
//                                 {"花","落","知","多","少"}};

//System.Console.WriteLine("---横版---");
//for (int i = 0; i < 4; i++)
//{
//    for(int j = 0; j < 5; j++)
//    {
//        System.Console.Write(poem[i,j]);
//    }
//    System.Console.WriteLine();
//}

//System.Console.WriteLine("\n--竖版--");
//for(int i = 0; i <5; i++)
//{

//    for (int j = 3; j >=0; j--)
//    {
//        System.Console.Write(poem[j, i]);
//    }
//    System.Console.WriteLine();
//}


//class Program
//{
//    private int Add(int x , int y) 
//    { 
//        return x + y;
//    }
//}
//class MainApp
//{
//    static void Main(string[] args)
//    {
//        Program p = new Program();

//        System.Console.WriteLine();
//    }
//}

List<int> a = new List<int>(); 
ArrayList b = new ArrayList();

b.Add("q");
b.Add('a');
b.Add(1);
foreach(var i in b)
{
    System.Console.WriteLine(i);
}
double[] s = new double[] { 1, 2, 3.12 };
Sale sa = new Sale();
System.Console.WriteLine(sa.SaleMoney(s));
class Sale
{
    public double SaleMoney<T>(T[] items)
    {
        double sum = 0;
        foreach(T item in items)
        {
            sum+=Convert.ToDouble(item);
        }
        return sum;
    }
}
