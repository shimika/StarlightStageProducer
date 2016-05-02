using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StarlightStageProducer {
	class Network {
		public static string DataEndPoint = "http://imas.inven.co.kr/dataninfo/card/sl_list.php";
		public static string InfoEndPoint = "http://imas.inven.co.kr/dataninfo/layer.xml.php?code";

		private static int Delay = 10;

		private Random random;
		private bool download = false;
		private List<Idol> idols;
		public event EventHandler<DataEventArgs> Completed;
		public event EventHandler<LoadingEventArgs> Loading;

		public enum Status { OK, Error, Cached };

		public Network() {
			random = new Random();
		}

		public void Download() {
			if (download) { return; }
			download = true;

			FileSystem.CheckDirectory();
			idols = new List<Idol>();

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += Worker_DoWork;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			worker.RunWorkerAsync();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e) {
			SendLoadingStatus("Database downloading...");

			string html = GET(DataEndPoint);
			idols = Parser.ParseHtml(html, this);

			bool success = true;

			for (int i = 0; i < idols.Count; i++) {
				Idol idol = idols[i];
				Status status = DownloadImage(idol.ImageUrl, idol.Id);
				if (DownloadImage(idol.ImageUrl, idol.Id) == Status.Error) {
					success = false;
					break;
				}

				SendLoadingStatus(string.Format("Image downloading... {0} / {1}", i + 1, idols.Count));

				if (status == Status.OK) {
					Thread.Sleep(Delay);
				}
			}

			if (!success) { idols = null; }
		}

		public void SendLoadingStatus(string status) {
			if (Loading != null) {
				Loading(this, new LoadingEventArgs(status));
			}
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (Completed != null) {
				Completed(this, new DataEventArgs(idols));
			}
		}

		public static Status DownloadImage(string url, int id) {
			string path = string.Format("{0}{1}.jpg", FileSystem.DataFolder, id);
			if (File.Exists(path)) {
				return Status.Cached;
			}

			Random random = new Random();
			string tempPath = string.Format("{0}{1}.jpg", FileSystem.TempFolder, random.Next().ToString("X"));

			WebClient client = new WebClient();
			client.Proxy = null;

			try { client.DownloadFile(url, tempPath); }
			catch (Exception ex) {
				//Console.WriteLine(ex.Message);
				return Status.Error;
			}

			File.Move(tempPath, path);		
			return Status.OK;
		}

		public static string GET(string url) {
			Thread.Sleep(Delay);

			try {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new UriBuilder(url).Uri);
				httpWebRequest.Accept = "*/*";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
				httpWebRequest.Method = "GET";
				httpWebRequest.Referer = "google.com";
				httpWebRequest.UserAgent =
					"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; WOW64; " +
					"Trident/4.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; " +
					".NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET CLR 3.0.30618; " +
					"InfoPath.2; OfficeLiveConnector.1.3; OfficeLivePatch.0.0)";
				httpWebRequest.Proxy = null;

				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

				return streamReader.ReadToEnd();
			}
			catch (Exception ex) {
				//MessageBox.Show(ex.Message);
				//Console.WriteLine(ex.Message);
			}

			return null;
		}
	}

	public class DataEventArgs : EventArgs {
		public List<Idol> Idols { get; internal set; }

		public DataEventArgs(List<Idol> idols) {
			this.Idols = idols;
		}
	}

	public class LoadingEventArgs : EventArgs {
		public string Status { get; internal set; }

		public LoadingEventArgs(string status) {
			this.Status = status;
		}
	}
}
