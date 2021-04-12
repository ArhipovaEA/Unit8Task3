using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Unit8Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            string PathDel;
            double SizeAllbefor = 0;
            double SizeAllafter = 0;
            int LenPath;
            do
            {
                Console.WriteLine("Укажите путь по которому необходимо произвести удаление данных: ");
                PathDel = Console.ReadLine();
                LenPath = PathDel.Length;
            }
            while (LenPath < 3);

            CalkSize(PathDel, ref SizeAllbefor);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Размер указанного каталога:" + SizeAllbefor);
            Console.ResetColor();
            
            int DelCount = 0;
            DeleteAll(PathDel, ref DelCount);
            CalkSize(PathDel, ref SizeAllafter);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Размер указанного каталога после удаления:" + SizeAllafter);
            Console.WriteLine("Удалено файлов:" + DelCount);
            Console.WriteLine("Освобождено места:" + (SizeAllbefor -SizeAllafter));


            Console.ResetColor();
        }

        static double CalkSize(string PathForCalk, ref double SizeCalk)
        {
            if (Directory.Exists(PathForCalk))
            {
               
                try
                {
                    DirectoryInfo di = new DirectoryInfo(PathForCalk);

                    FileInfo[] fileNames = di.GetFiles();

                    foreach (FileInfo f in fileNames)
                    {
                        
                        SizeCalk = SizeCalk + f.Length;
                    }

                    DirectoryInfo[] folderInfo = di.GetDirectories();
                    foreach (DirectoryInfo df in folderInfo)
                    {
                        CalkSize(df.FullName, ref SizeCalk);
                    }

                    return Math.Round((double)(SizeCalk / 1024 / 1024 / 1024), 1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                    return 0;
                }

            }
            else
            {
                Console.WriteLine("Данная директория не существует - " + PathForCalk);
                return 0;
            }


        }

        static void DelFile(string filePath, int MinDel)
        {
            TimeSpan interval;

            if (File.Exists(filePath))
            {
                interval = DateTime.Now - File.GetLastAccessTime(filePath);

                if (interval.TotalMinutes > MinDel)
                {
                    if (ChekAccessUser(filePath))
                    {
                        File.Delete(filePath);
                      }
                    else
                    {
                        Console.WriteLine("Нет прав на удаление " + filePath);
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл не существует - " + filePath);
            }
        }

        static void DeleteAll(string PathDel, ref int countdel)
        {
            if (Directory.Exists(PathDel))
            {
                Console.WriteLine("Удаление началась... " + PathDel);
                try
                {
                    var folderInfo = Directory.GetDirectories(PathDel);
                    for (int i = 0; i < folderInfo.Length; i++)
                    {
                        if (ChekAccessUser(folderInfo[i]))
                        {
                            DeleteAll(folderInfo[i], ref countdel);
                            DirectoryInfo dirInfo = new DirectoryInfo(folderInfo[i]);
                            if (dirInfo.GetFiles().Length == 0)
                            {
                                Directory.Delete(folderInfo[i]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Нет прав на удаление " + folderInfo[i]);
                        }
                    }

                    var fileNames = Directory.GetFiles(PathDel);

                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        DelFile(fileNames[i], 30);
                        countdel = countdel + 1;
                    }




                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                Console.WriteLine("Обработка завершилась... " + PathDel);
            }
            else
            {
                Console.WriteLine("Данная директория не существует - " + PathDel);
            }
        }

        static bool ChekAccessUser(string PathCheck)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(PathCheck);
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                DirectorySecurity ds = dir.GetAccessControl(AccessControlSections.Access);
                AuthorizationRuleCollection rules = ds.GetAccessRules(true, true, typeof(SecurityIdentifier));
                foreach (FileSystemAccessRule rl in rules)
                {
                    if (((rl.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete))
                    {
                        if (rl.AccessControlType == AccessControlType.Allow)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
                return false;
            }
        }

    }


}
