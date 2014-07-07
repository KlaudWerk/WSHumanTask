/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using log4net;

namespace HumanTask
{
    /// <summary>
    /// Logger extension utility class
    /// </summary>
    public static class LoggerExt
    {
        public static void DebugEx(this ILog logger, Func<string> f)
        {
            if(logger.IsDebugEnabled)
                logger.Debug(f.Invoke());
        }
        public static void WarnEx(this ILog logger, Func<string> f)
        {
            if (logger.IsWarnEnabled)
                logger.Debug(f.Invoke());

        }
        public static void InfoEx(this ILog logger, Func<string> f)
        {
            if (logger.IsInfoEnabled)
                logger.Debug(f.Invoke());

        }

        public static void ErrorEx(this ILog logger, Func<string> f)
        {
            if (logger.IsErrorEnabled)
                logger.Debug(f.Invoke());

        }
        public static void FatalEx(this ILog logger, Func<string> f)
        {
            if (logger.IsFatalEnabled)
                logger.Debug(f.Invoke());
           
        }

    }
}