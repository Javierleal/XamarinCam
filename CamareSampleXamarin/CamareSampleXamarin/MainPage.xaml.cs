using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net;
using System.IO;
using Microsoft.WindowsAzure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace CamareSampleXamarin
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();


            ImageSourse = new UriImageSource();
            ImageSourse.Uri = new Uri("https://catalogblog.blob.core.windows.net/imagecatalog/636366638468071910Prueba.png");
            FotoTomada.Source = ImageSourse;
        }

        public UriImageSource ImageSourse { get; set; }

        private async void TomarFoto_Clicked(object sender, EventArgs e)
		{
			await CrossMedia.Current.Initialize();

			if (CrossMedia.Current.IsCameraAvailable)
			{
				StoreCameraMediaOptions Options = new StoreCameraMediaOptions();
				Options.SaveToAlbum = true;
				Options.Name = string.Format("{0}Prueba.PNG",DateTime.Now.Ticks.ToString());
				var file = await CrossMedia.Current.TakePhotoAsync(Options);

				if (file != null)
				{
				    FotoTomada.Source = ImageSource.FromStream(() => file.GetStream());
                    string FileUrl = await UploadFileAsync("imagecatalog", Options.Name, file.GetStream());
				}
			}
		}


        static CloudBlobContainer GetContainer(string containerName)
        {
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=catalogblog;AccountKey=ljApxQInr4/Dsg5WY1Jbs5zrPeI1G6flS5rRhF57T87PH11AZDmDmE7tAJSxzJTchwNA4G+ZaZZ9ZJlPfAo60g==;EndpointSuffix=core.windows.net");
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerName);
        }

        public static async Task<string> UploadFileAsync(string containerName,string ImageName, Stream stream)
        {
            var container = GetContainer(containerName);
            await container.CreateIfNotExistsAsync();
            var fileBlob = container.GetBlockBlobReference(ImageName);
            await fileBlob.UploadFromStreamAsync(stream);
            return string.Format("https://catalogblog.blob.core.windows.net/{0}/{1}", containerName, ImageName);
        }

    }
}
