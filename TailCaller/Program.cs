using System;
using System.Numerics;

namespace TailCaller
{
    public abstract class TailCall<T> {

        public abstract TailCall<T> Resume();

        public abstract T Eval();

        public abstract bool IsSuspend();

        private TailCall() {}

        public class Return<T> : TailCall<T> {

            private readonly T t;

            internal Return(T t)
            {
                this.t = t;
            }


            public override T Eval() => t;
            public override bool IsSuspend()
            {
                return false;
            }

            public override TailCall<T> Resume()
            {
                throw new Exception("Return has no resume");
            }
        }

        public  class Suspend<T> : TailCall<T> {

            private readonly Func<TailCall<T>> _resume;

            internal Suspend(Func<TailCall<T>> resume)
            {
                _resume = resume;
            }

            public override T Eval() {
                TailCall<T> tailRec = this;

                while(tailRec.IsSuspend()) {
                    tailRec = tailRec.Resume();
                }
                return tailRec.Eval();
            }

            public override bool IsSuspend()
            {
                return true;
            }

            public override TailCall<T> Resume()
            {
                return _resume.Invoke();
            }
        }

        public static Return<T> Ret(T t)
        {
            return new Return<T>(t);
        }

        public static Suspend<T> Sus(Func<TailCall<T>> s)
        {
            return new Suspend<T>(s);
        }
    }

    internal class Program
    {
        public static BigInteger Sum(BigInteger arg) {
            return Sum(arg, 0).Eval();
        }

        private static TailCall<BigInteger> Sum(BigInteger arg, BigInteger acc) {
            return arg == 0
                ? (TailCall<BigInteger>) TailCall<BigInteger>.Ret(acc)
                : TailCall<BigInteger>.Sus(() => Sum(arg - 1, acc + arg));
        }
        public static void Main(string[] args)
        {
            BigInteger sum = Sum(500000000000);
            Console.WriteLine(sum);
        }
    }
}