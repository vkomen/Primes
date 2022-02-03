using System;
using System.Collections.Generic;

namespace Simple
{
    class Program {

        public struct fraction {
            public bool sign;
            public long numenator;
            public long denomenator;
            public bool prime;
        }
        public struct stat {
            public long prime;
            public long complex;
            public long minval;
            public long maxval;
            public long maxprime;
        }

        //      Сложение 2-х дробей
        static fraction fAdd(fraction f1, fraction f2) {
            fraction f;
            f.numenator = f1.numenator * f2.denomenator + f2.numenator * f1.denomenator;
            f.denomenator = f1.denomenator * f2.denomenator;
            f.sign = true;
            f.prime = false;
            return Simplify(f);
        }

        //      Вычитание 2-х дробей
        static fraction fSub(fraction f1, fraction f2) {
            fraction f;
            f.numenator = f1.numenator * f2.denomenator - f2.numenator * f1.denomenator;
            f.denomenator = f1.denomenator * f2.denomenator;
            f.sign = (f.numenator > 0);
            f.prime = false;
            return Simplify(f);
        }

        //      Сокращение дроби, приведение
        static fraction Simplify(fraction f) {
            fraction sf;
            long NOD = EuclidNOD(Math.Abs(f.numenator), f.denomenator);
            sf = f;
            if (NOD > 1) {
                sf.numenator = f.numenator / NOD;
                sf.denomenator = f.denomenator / NOD;
            }
            return sf;
        }

        //      Наибольший общий делитель алгоритмом Евклида
        static long EuclidNOD(long n1, long n2) {
            long n;
            while (n1 != n2)
            {
                if (n1 > n2) { n = n2; n2 = n1; n1 = n; }
                n2 = n2 - n1;
            }
            return n1;
        }

        //      Первое слагаемое произведения sin(a)*sin(b) = cos(a-b) ...
        static fraction sinsin1(fraction f1, fraction f2, Boolean sign1) {
            fraction f;
            f = fSub(f1, f2);
            f.numenator = Math.Abs(f.numenator);
            f.sign = !(true ^ sign1);
            f.prime = IsPrime(f.numenator);
            return f;
        }

        //      Второе слагаемое произведения sin(a)*sin(b)= ... - cos(a+b)
        static fraction sinsin2(fraction f1, fraction f2, Boolean sign1) {
            fraction f;
            f = fAdd(f1, f2);
            f.sign = !(false ^ sign1);
            f.prime = IsPrime(f.numenator);
            return f;
        }

        //      Первое слагаемое произведения cos(a)*sin(b) = sin(b-a) ...
        static fraction cossin1(fraction f1, fraction f2, Boolean sign1) {
            fraction f;
            f = fSub(f2, f1);
            f.numenator = Math.Abs(f.numenator);
            f.sign = !(f.sign ^ sign1);
            f.prime = IsPrime(f.numenator);
            return f;
        }

        //      Второе слагаемое произведения cos(a)*sin(b) = ... + sin(b+a)
        static fraction cossin2(fraction f1, fraction f2, Boolean sign1) {
            fraction f;
            f = fAdd(f1, f2);
            f.sign = !(true ^ sign1);
            f.prime = IsPrime(f.numenator);
            return f;
        }

        //      Насчет массива fractions на основе предыдущей итерации (домножение на очередной множитель-синус)
        //      Для n = 1 - просто инициализация массива дробью 1/2
        static List<fraction> Iteration(List<fraction> res, int n) {

            int[] series = { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199 };
            // int[] series = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30 };

            fraction nextf;
            nextf.numenator = 1;
            nextf.denomenator = series[n];
            nextf.sign = true;
            nextf.prime = true;

            List<fraction> newres = new List<fraction>(0);

            if (n == 1) {
                newres.Add(nextf);
                return newres;
            }

            if ((n % 2) == 0) {
                for (var i = 0; i < res.Count; i++) {
                    newres.Add(sinsin1(res[i], nextf, res[i].sign));
                    newres.Add(sinsin2(res[i], nextf, res[i].sign));
                }
            }
            else {
                for (var i = 0; i < res.Count; i++)
                {
                    newres.Add(cossin1(res[i], nextf, res[i].sign));
                    newres.Add(cossin2(res[i], nextf, res[i].sign));
                }
            }
            return newres;
        }

        //      Проверка на простоту самым простым способом
        static bool IsPrime(long N) {
            if (N < 4) { return true; }
            if ((N % 2) == 0) { return false; }

            for (var m = 3; m < Math.Sqrt(N) + 1; m += 2)
            {
                if ((N % m) == 0) { return false; }
            }
            return true;
        }

        //      Проверяет, сколько простых и непростых в переданном массиве. 
        static stat Statistics(List<fraction> res) {
            stat PrimeComplex;
            PrimeComplex.prime = 0;
            PrimeComplex.complex = 0;
            PrimeComplex.maxval = 0;
            PrimeComplex.minval = 9223372036854775807;
            PrimeComplex.maxprime = 0;

            for (var i = 0; i < res.Count; i++) {
                if (res[i].prime)
                {
                    PrimeComplex.prime++;
                    if (Math.Abs(res[i].numenator) > PrimeComplex.maxprime) { PrimeComplex.maxprime = Math.Abs(res[i].numenator); }
                }
                else {
                    PrimeComplex.complex++;
                }
                if (Math.Abs(res[i].numenator) > PrimeComplex.maxval) { PrimeComplex.maxval = Math.Abs(res[i].numenator); }
                if (Math.Abs(res[i].numenator) < PrimeComplex.minval) { PrimeComplex.minval = Math.Abs(res[i].numenator); }
            }
            return PrimeComplex;
        }

//      Функция возвращает свой статус "простой/составной" и при недостижении последнего уровня уходит в рекурсию
        static long AnalyzeOne(int mylevel, int myorder, int maxlevel, List<List<fraction>> AllRes)
        {
            if (mylevel == maxlevel) {
                return AllRes[mylevel][myorder].prime ? 1 : 0; 
            }
            else { 
                return (AllRes[mylevel][myorder].prime ? 1 : 0) + 
                       AnalyzeOne(mylevel + 1, myorder * 2,  maxlevel, AllRes)+
                       AnalyzeOne(mylevel + 1, myorder * 2 + 1, maxlevel, AllRes); 
            }
        }

        static void Main(string[] args) {
            stat PrimeComplex;
            long s;

            List<fraction> Result =  new List<fraction>();
            List<List<fraction>> AllRes = new List<List<fraction>>();

            Console.WriteLine("Hello World!");

            for (var i = 1; i < 16; i++) 
            {
                Result = Iteration(Result, i);
                PrimeComplex = Statistics(Result);
                
                Console.WriteLine(DateTime.Now + "   Iteration #" + i );
                Console.WriteLine("Primes: " + PrimeComplex.prime + " , Complex: " + PrimeComplex.complex + " , MAX PRIME = " + PrimeComplex.maxprime + " , MIN = " + PrimeComplex.minval + " , MAX = " + PrimeComplex.maxval);

                AllRes.Add(Result);
            }

            for (var i = 0; i < 8; i++)
            {
                Console.WriteLine("Offsprings of " + AllRes[3][i].numenator +  ": " + AnalyzeOne(3, i, 14, AllRes));
            }

            for (var i = 0; i < 12; i++)
            {
                s = 0;
                for (var j = AllRes[i].Count/2; j < AllRes[i].Count; j++)
                {
                    s += Math.Abs(AllRes[i][j].numenator);
                }
                Console.WriteLine("Iteration " + (i + 2) + ", SUM is " + s);
            }
        }
    }
}


