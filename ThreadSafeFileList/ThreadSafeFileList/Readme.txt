Copyright Philip Pierce © 2010-2014
 
A recent project I worked on required me to retrieve a large listing of files and 
folders from a given computer. My original idea was to use the built in 
Directory.GetDirectories and Directory.GetFiles.

However, as I soon found out, there is a problem with both of these functions, 
especially when used in Windows 7 and 8. These functions will error 
out (and return an empty array), if any of the folders/files are not accessible to 
the user. So, using Directory.GetDirectories("C:\\"), would return an empty array
and an exception, depending on the user’s security

The main problem comes from how some folders and files are ACL’d in Windows 7 and 8.
Most users (including admin), cannot parse the path of some ACL’d folders, even if 
they have permission.

My solution was to create a class which can handle the security errors and 
still return all the files as needed. In addition, I wanted to make this as 
extensible as possible, so I also added a Predicate to each function, which 
allows you create your own filter.

example:
Get a list of all "jpg" files on the "C:" drive
    TList<string> JpgFolders = DirectoryList(@"C:\\", true, x => 
      (Path.GetExtension(x) == ".jpg"));

Because I use this heavily in multi-threaded environments, I have it returning the 
lists in a thread safe manner, using the thread safe List<> collection I wrote (TList).

Also, because of the number of files and folders being returned (sometimes in the hundreds 
of thousands or millions), I decided to use PLINQ to help speed things up. PLINQ allows 
me to branch each ForEach loop to a new thread, if a thread is available; otherwise, 
it will wait for an available thread. PLINQ can also send multiple iterations of the 
loop to one thread, which also helps to improve efficiency.

You’ll also notice the use multiple catch blocks. I found that two types of exceptions 
were raised if a user did not have access to a file/folder – IOException and 
UnauthorizedAccessException. There is no reason to do anything when these exceptions 
are raised, since it simply means the user does not have permission for this file/folder, 
so we just need to skip it.
