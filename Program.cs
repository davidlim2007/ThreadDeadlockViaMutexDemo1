using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadDeadlockViaMutexDemo01
{
    class Program
    {
        static void Main(string[] args)
        {
            StartThread01();
            StartThread02();
            WaitThread01ToEnd();
            WaitThread02ToEnd();
        }

        static void StartThread01()
        {
            m_thread_01 = new Thread(new ThreadStart(ThreadMethod01));
            m_thread_01.Start();
        }

        static void StartThread02()
        {
            m_thread_02 = new Thread(new ThreadStart(ThreadMethod02));
            m_thread_02.Start();
        }
        
        static void ThreadMethod01()
        {
            try
            {
                // Acquire m_mutex_01 immediately.
                m_mutex_01.WaitOne();

                // Do some activity.
                // This activity (putting this thread to sleep for 2 secs)
                // is such that it allows ThreadMethod02 to start and so 
                // m_mutex_02 becomes acquired by ThreadMethod02.
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Before being able to release m_mutex_01,
                // this thread must first acquire m_mutex_02.
                //
                // Deadlock will occur because m_mutex_02 is
                // currently acquired by ThreadMethod02, which in turn
                // is waiting for ThreadMethod01 to release m_mutex_01.
                //
                // Hence, neither thread is able to proceed.
                m_mutex_02.WaitOne();
                m_mutex_01.ReleaseMutex();
                m_mutex_02.ReleaseMutex();
            }
        }

        static void ThreadMethod02()
        {
            try
            {
                // Acquire m_mutex_02 immediately.
                m_mutex_02.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Before being able to release m_mutex_02,
                // this thread must first acquire m_mutex_01.
                //
                // Deadlock will occur because m_mutex_01 is
                // currently acquired by ThreadMethod01, which in turn
                // is waiting for ThreadMethod02 to release m_mutex_02.
                //
                // Hence, neither thread is able to proceed.
                m_mutex_01.WaitOne();
                m_mutex_02.ReleaseMutex();
                m_mutex_01.ReleaseMutex();
            }
        }

        static void WaitThread01ToEnd()
        {
            m_thread_01.Join();
        }

        static void WaitThread02ToEnd()
        {
            m_thread_02.Join();
        }

        private static Thread m_thread_01 = null;
        private static Thread m_thread_02 = null;
        private static Mutex m_mutex_01 = new Mutex(false);
        private static Mutex m_mutex_02 = new Mutex(false);
    }
}