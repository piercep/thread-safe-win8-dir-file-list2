using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadSafeFileList
{
    /// <summary>
    /// Handles building file and directory lists for Windows 7, 8, and beyond
    /// </summary>
    public static class DirectoryListing
    {
        #region DirectoryList

        /// <summary>
        /// Returns a list of directories under RootDirectory
        /// </summary>
        /// <param name="RootDirectory">starting directory</param>
        /// <param name="SearchAllDirectories">when true, all sub directories will be searched as well</param>
        /// <param name="Filter">filter to be done on directory. use null for no filtering</param>
        public static TList<string> DirectoryList(string RootDirectory, bool SearchAllDirectories, Predicate<string> Filter)
        {
            TList<string> retList = new TList<string>();

            try
            {
                // create a directory info object
                DirectoryInfo di = new DirectoryInfo(RootDirectory);

                // loop through directories populating the list
                Parallel.ForEach(di.GetDirectories(), folder =>
                    {
                        try
                        {
                            // add the folder if it passes the filter
                            if ((Filter == null) || (Filter(folder.FullName)))
                            {
                                // add the folder
                                retList.Add(folder.FullName);

                                // get it's sub folders
                                if (SearchAllDirectories)
                                    retList.AddRange(DirectoryList(folder.FullName, true, Filter));
                            }
                        }

                        catch (IOException)
                        {
                            // don't really need to do anything
                            // user just doesn't have access
                        }

                        catch (UnauthorizedAccessException)
                        {
                            // don't really need to do anything
                            // user just doesn't have access
                        }

                        catch (Exception excep)
                        {
                            // TODO: save to an error log somewhere

                            // for now, write to console
                            Console.WriteLine("Error: {0}", excep.Message);
                        }
                    });
            }

            catch (IOException)
            {
                // don't really need to do anything
                // user just doesn't have access
            }

            catch (UnauthorizedAccessException)
            {
                // don't really need to do anything
                // user just doesn't have access
            }

            catch (Exception excep)
            {
                // TODO: save to an error log somewhere

                // for now, write to console
                Console.WriteLine("Error: {0}", excep.Message);
            }

            finally
            {
                // force garbage collection
                GC.Collect();
                Thread.Sleep(10);
            }

            // return the list
            return retList;
        }

        // DirectoryList
        #endregion

        #region FileList

        /// <summary>
        /// Returns a list of files under RootDirectory
        /// </summary>
        /// <param name="RootDirectory">starting directory</param>
        /// <param name="SearchAllDirectories">when true, all sub directories will be searched as well</param>
        /// <param name="Filter">filter to be done on files/directory. use null for no filtering</param>
        public static TList<string> FileList(string RootDirectory, bool SearchAllDirectories, Predicate<string> Filter)
        {
            TList<string> retList = new TList<string>();

            try
            {
                // get the list of directories
                TList<string> DirList = new TList<string> { RootDirectory };

                // get sub directories if allowed
                if (SearchAllDirectories)
                    DirList.AddRange(DirectoryList(RootDirectory, true, null));

                // loop through directories populating the list
                Parallel.ForEach(DirList, folder =>
                    {
                        // get a directory object
                        DirectoryInfo di = new DirectoryInfo(folder);

                        try
                        {
                            // loop through the files in this directory
                            foreach (FileInfo file in di.GetFiles())
                            {
                                try
                                {
                                    // add the file if it passes the filter
                                    if ((Filter == null) || (Filter(file.FullName)))
                                        retList.Add(file.FullName);
                                }

                                catch (IOException)
                                {
                                    // don't really need to do anything
                                    // user just doesn't have access
                                }

                                catch (UnauthorizedAccessException)
                                {
                                    // don't really need to do anything
                                    // user just doesn't have access
                                }

                                catch (Exception excep)
                                {
                                    // TODO: save to an error log somewhere

                                    // for now, write to console
                                    Console.WriteLine("Error: {0}", excep.Message);
                                }
                            }
                        }

                        catch (IOException)
                        {
                            // don't really need to do anything
                            // user just doesn't have access
                        }

                        catch (UnauthorizedAccessException)
                        {
                            // don't really need to do anything
                            // user just doesn't have access
                        }


                        catch (Exception excep)
                        {
                            // TODO: save to an error log somewhere

                            // for now, write to console
                            Console.WriteLine("Error: {0}", excep.Message);
                        }
                    });
            }

            catch (IOException)
            {
                // don't really need to do anything
                // user just doesn't have access
            }

            catch (UnauthorizedAccessException)
            {
                // don't really need to do anything
                // user just doesn't have access
            }

            catch (Exception excep)
            {
                // TODO: save to an error log somewhere

                // for now, write to console
                Console.WriteLine("Error: {0}", excep.Message);
            }

            finally
            {
                // force garbage collection
                GC.Collect();
                Thread.Sleep(10);
            }

            // return the list
            return retList;
        }

        // FileList
        #endregion
    }
}