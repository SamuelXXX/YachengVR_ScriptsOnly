using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Electrics.Utility
{
    /// <summary>
    /// The operation of complex number
    /// </summary>
    [System.Serializable]
    public struct Complex
    {
        #region Properties
        public float re;
        public float im;
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(re * re + im * im);
            }
            set
            {
                var s = Mathf.Sqrt(re * re + im * im);
                if (s == 0)
                {
                    re = value;
                }
                else
                {
                    re *= value / s;
                    im *= value / s;
                }
            }
        }

        public float argument
        {
            get
            {
                if (re == 0)
                {
                    if (im > 0)
                    {
                        return Mathf.PI / 2f;
                    }
                    else if (im < 0)
                    {
                        return -Mathf.PI / 2f;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else if (re > 0)
                {
                    return Mathf.Atan(im / re);
                }
                else
                {
                    return Mathf.Atan(im / re) + Mathf.PI;
                }
            }
            set
            {
                var s = magnitude;
                re = s * Mathf.Cos(value);
                im = s * Mathf.Sin(value);
            }
        }
        #endregion

        #region Constructor
        public Complex(float re, float im)
        {
            this.re = re;
            this.im = im;
        }
        #endregion

        #region Operations
        public void SetZero()
        {

            re = 0;

            im = 0;

        }

        public static Complex operator +(Complex a, Complex b)
        {
            Complex ret = new Complex();
            ret.re = a.re + b.re;
            ret.im = a.im + b.im;
            return ret;
        }

        public static Complex operator -(Complex a, Complex b)
        {
            Complex ret = new Complex();
            ret.re = a.re - b.re;
            ret.im = a.im - b.im;
            return ret;
        }

        public static Complex operator *(Complex a, Complex b)
        {
            Complex ret = new Complex();
            ret.re = a.re * b.re - a.im * b.im;
            ret.im = a.im * b.re + a.re * b.im;
            return ret;
        }

        public static Complex operator *(Complex a, float n)
        {
            Complex ret = new Complex();
            ret.re = a.re * n;
            ret.im = a.im * n;
            return ret;
        }

        public static Complex operator *(float n, Complex a)
        {
            Complex ret = new Complex();
            ret.re = a.re * n;
            ret.im = a.im * n;
            return ret;
        }

        public static Complex operator /(Complex a, float n)
        {
            if (n == 0)
            {
                throw new System.DivideByZeroException("Complex divide zero");
            }
            Complex ret = new Complex();
            ret.re = a.re / n;
            ret.im = a.im / n;
            return ret;
        }

        public static Complex operator /(Complex a, Complex b)
        {
            if (b.IsZero())
            {
                throw new System.DivideByZeroException("Complex divide zero");
            }
            return a * b.conjugate() / (b.re * b.re + b.im * b.im);
        }

        public static float Dot(Complex a, Complex b)
        {
            return a.re * b.re + a.im * b.im;
        }

        public Complex Project(Complex t)
        {
            if (t.IsZero())
            {
                throw new System.DivideByZeroException("Complex divide zero");
            }
            return Complex.Dot(this, t) * t / (t.re * t.re + t.im * t.im);
        }

        public bool Equal(Complex t)
        {
            return re == t.re && im == t.im;
        }

        public Complex conjugate()
        {
            Complex ret = new Complex();
            ret.re = re;
            ret.im = -im;
            return ret;
        }

        public bool IsZero()
        {
            return re == 0 && im == 0;
        }
        #endregion
    }
}

