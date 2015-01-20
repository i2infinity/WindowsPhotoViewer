using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Models;

namespace PhotoLoaderSDK
{
    public interface IFilePicker<T>
    {
        /// <summary>
        /// Filter on file types 
        /// </summary>
        /// <param name="fileType"></param>
        void AddFilter(String fileType);

        /// <summary>
        /// Pick a list of files
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<T>> PickFiles(Boolean allowMultiple = true);
    }
}