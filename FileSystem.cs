﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;


namespace ToolSL
{
    public static class FileSystem
    {
        public static string AutofindFolder(string path)
        {            
            if (Alphaleonis.Win32.Filesystem.File.Exists(path))
            {
                var lines = Alphaleonis.Win32.Filesystem.File.ReadAllLines(path);
                var line = lines.FirstOrDefault(item => item.StartsWith("MoveProcessedFilesDirectory="));
                var dir = line.Replace("MoveProcessedFilesDirectory=","");
                return dir;
            }
            return null;
        }
    }
}