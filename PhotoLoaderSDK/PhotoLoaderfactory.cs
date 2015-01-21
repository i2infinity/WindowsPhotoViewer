using System;
using PhotoLoader.Models;
using PhotoLoaderSDK.CloudPhotoLoaders;
using PhotoLoaderSDK.LocalStoragePhotoLoader;

namespace PhotoLoaderSDK
{
    public class PhotoLoaderFactory
    {
        public IPhotoRetriever GetPhotoRetriever(PhotoDataSource dataSource)
        {
            switch (dataSource)
            {
                case PhotoDataSource.LocalStorage:
                    return new LocalStorageLoader(new LocalStorageFilePicker());

                case PhotoDataSource.Facebook:
                    return new FacebookLoader();

                case PhotoDataSource.Flickr:
                   return new FlickrLoader();

                case PhotoDataSource.Instagram:
                    return new InstagramLoader();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
