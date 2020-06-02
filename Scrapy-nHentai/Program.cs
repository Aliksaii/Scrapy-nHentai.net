using System;
using static System.Console;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Net;

namespace Scrapy_nHentai
{
	class Program
	{
		static void Main(string[] args)
		{
			Title = "nHentai.net";
			OutputEncoding = System.Text.Encoding.Unicode;
			HtmlWeb Data = new HtmlWeb();
			List<string> Titles = new List<string>();
			List<string> Codes = new List<string>();
			List<string> Tags = new List<string>();
			string URL = "https://www.nhentai.net";
			bool Exit = false;
			do
			{
				int Option = Main_Menu();
				Clear();
				switch (Option)
				{
					case 1:
						SearchByPage(ref URL, ref Data, ref Titles, ref Codes, ref Tags);
						break;
					case 2:
						SearchByCode(ref URL, ref Data, ref Titles, ref Tags, ref Codes, false);
						break;
					case 3:
						SearchByCode(ref URL, ref Data, ref Titles, ref Tags, ref Codes, true);
						break;
					case 4:
						Exit = true;
						break;
				}
			}
			while (Exit != true);
		}
		static int Main_Menu()
		{
			int Selected_Option = 0;
			bool flag = true;
			do
			{
				Clear();
				CursorVisible = true;
				// Maybe i'll change this using --> SetCursorPosition()
				ForegroundColor = ConsoleColor.DarkRed;
				Write("   ╔════════════════════════╗\n   ║  ");
				ForegroundColor = ConsoleColor.Gray;
				Write("__ |_| _ __ _|_ _  o");
				ForegroundColor = ConsoleColor.DarkRed;
				Write("  ║\n   ║  ");
				ForegroundColor = ConsoleColor.Gray;
				Write("| || |(/_| | |_(_| |  ");
				ForegroundColor = ConsoleColor.DarkRed;
				WriteLine("║\n   ╚════════════════════════╝");
				BackgroundColor = ConsoleColor.DarkRed;
				ForegroundColor = ConsoleColor.White;
				Write(">> Options:\n".ToUpper());
				BackgroundColor = ConsoleColor.Black;
				Write("\n[1]-->[ Search content in page ]".ToUpper() +
					  "\n[2]-->[ Search by code         ]".ToUpper() +
					  "\n[3]-->[ Randomize code search  ]".ToUpper() +
					  "\n[4]-->[ Exit this program :/   ]".ToUpper() +
					  "\n\n[Select option]--> ".ToUpper());
				ForegroundColor = ConsoleColor.Yellow;
				try { Selected_Option = int.Parse(ReadLine()); flag = false; }
				catch (FormatException e)
				{
					CursorVisible = false;
					ForegroundColor = ConsoleColor.Red;
					WriteLine($"[\\Error]: {e.Message}".ToUpper());
					flag = true;
					ReadKey();
				}
			}
			while (flag == true || (Selected_Option < 1 || Selected_Option > 4));
			return Selected_Option;
		}
		static void WebData(ref HtmlDocument Reader, ref List<string> Titles, ref List<string> Codes)
		{
			foreach (var item in Reader.DocumentNode.CssSelect(".caption"))
			{
				int start = item.InnerText.ToString().IndexOf("] ", 0), finish;
				if (start < 0) { finish = item.InnerText.ToString().IndexOf(" [", 0); }
				else { finish = item.InnerText.ToString().IndexOf(" [", start); }
				int lenght = finish - start;
				if (lenght > 0)
				{
					try { Titles.Add(item.InnerText.ToString().Substring(start + 2, lenght - 2)); }
					catch { Titles.Add(item.InnerText); }
				}
				else
				{
					Titles.Add(item.InnerText);
				}
			}
			foreach (var item in Reader.DocumentNode.CssSelect(".gallery"))
			{
				var rawCode = item.CssSelect("a").First();
				Codes.Add(rawCode.GetAttributeValue("href"));
			}
		}
		static void ShowWebContent(ref int PageNumber, ref List<string> Titles, ref int respuesta)
		{
			BackgroundColor = ConsoleColor.DarkRed;
			ForegroundColor = ConsoleColor.White;
			WriteLine($"[ nHentai.net | Page: {PageNumber} ]".ToUpper());
			ResetColor();
			for (int i = 0; i < Titles.Count(); i++)
			{
				ForegroundColor = ConsoleColor.White;
				Write($"[{i + 1}] --> ");
				ForegroundColor = ConsoleColor.Gray;
				WriteLine($"{Titles[i].ToUpper()}");
			}
			ForegroundColor = ConsoleColor.Yellow;
			Write("Select doujinshi >> ".ToUpper());
			CursorVisible = true;
			ForegroundColor = ConsoleColor.White;
			try
			{
				respuesta = int.Parse(ReadLine());
				respuesta = respuesta - 1;
			}
			catch (FormatException e)
			{
				ForegroundColor = ConsoleColor.Red;
				CursorVisible = false;
				Write($"[\\Error]: {e.Message}".ToUpper());
				ForegroundColor = ConsoleColor.Green;
				Write($"\n[\\PS]: Enter --> 0 to return main menu".ToUpper());
				respuesta = -5;
				ReadKey();
			}
		}
		static void SearchByPage(ref string URL, ref HtmlWeb Data, ref List<string> Titles, ref List<string> Codes, ref List<string> Tags)
		{
			BackgroundColor = ConsoleColor.DarkBlue;
			ForegroundColor = ConsoleColor.White;
			WriteLine("[1]-->[ Search content in page ]\n".ToUpper());
			ResetColor();
			int respuesta = 0, PageNumber = 0;
			Write(">> Select page number: ".ToUpper());
			try { PageNumber = int.Parse(ReadLine()); }
			catch (FormatException e)
			{
				ForegroundColor = ConsoleColor.Red;
				CursorVisible = false;
				WriteLine($"[\\Error]: {e.Message}".ToUpper());
				Thread.Sleep(1500);
				ForegroundColor = ConsoleColor.Green;
				WriteLine(">> Selecting default page ---> [1]".ToUpper());
				Thread.Sleep(1250);
				PageNumber = 1;
			}
			CursorVisible = false;
			if (PageNumber > 1) { URL += $"/?page={PageNumber}"; }
			else { URL = "https://www.nhentai.net"; }
			ForegroundColor = ConsoleColor.White;
			WriteLine(">> Getting data from nHentai.net".ToUpper());
			HtmlDocument Reader = Data.Load(URL);
			WebData(ref Reader, ref Titles, ref Codes);
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine(">> Data obtained successfully!".ToUpper());
			ReadKey();
			do
			{
				URL = "https://www.nhentai.net";
				do
				{
					Clear();
					ShowWebContent(ref PageNumber, ref Titles, ref respuesta);
				}
				while (respuesta < -1 || respuesta > Titles.Count());
				if (respuesta >= 0)
				{
					Clear();
					Selected_H(ref respuesta, ref URL, ref Reader, ref Data, ref Titles, ref Codes, ref Tags);
					Console.ReadKey();
				}
				Tags.Clear();
			}
			while (respuesta >= 0);
			Titles.Clear();
			Codes.Clear();
		}
		static void Selected_H(ref int respuesta, ref string URL, ref HtmlDocument Reader, ref HtmlWeb Data, ref List<string> Titles, ref List<string> Codes, ref List<string> Tags)
		{
			CursorVisible = false;
			BackgroundColor = ConsoleColor.DarkBlue;
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine($"Title: {Titles[respuesta]}\n".ToUpper());
			ResetColor();
			Reader = Data.Load(URL += Codes[respuesta]);
			GetTitleInfo(ref Reader, ref Tags);
			ForegroundColor = ConsoleColor.White;
			string Code_Numbers = Codes[respuesta].ToString();
			Write(">> Code: ".ToUpper() + Code_Numbers.Substring(3, Code_Numbers.Length - 4)
				+ "\n>> Tags:\n");
			for (int i = 0; i < Tags.Count(); i++)
			{
				ForegroundColor = Tag_Color();
				Write($"\t[{Tags[i]}]\n");
			}
			ResetColor();
			Downloader(ref Reader, ref Data, URL + "1/", Code_Numbers.Substring(3, Code_Numbers.Length - 4));
		}
		private static ConsoleColor Tag_Color()
		{
			Random rand = new Random();
			int[] ColorNames = { 8, 7 };
			var ColorTags = ColorNames;
			return (ConsoleColor)ColorTags.GetValue(rand.Next(ColorTags.Length));
		}
		static void GetTitleInfo(ref HtmlDocument Reader, ref List<string> Tags)
		{
			int i = 0;
			foreach (var item in Reader.DocumentNode.CssSelect("span[class='tags']"))
			{
				// Coding skills 10/10
				switch (i)
				{
					// case 0 --> Parodies
					// case 1 --> Characters
					case 2: // <-- Tags
						foreach (var rawTags in item.CssSelect("a"))
						{
							Tags.Add(rawTags.InnerText);
						}
						break;
						// case 3 --> Artists
						// case 4 --> Groups
						// case 5 --> Languages
						// case 6 --> Categories
				}
				i++;
			}
		}
		static void GetTitleInfo(ref HtmlDocument Reader, ref List<string> Tags, ref List<string> Titles)
		{
			foreach (var item in Reader.DocumentNode.CssSelect("div[id='info']"))
			{
				int start = item.InnerText.ToString().IndexOf("] ", 0), finish;
				if (start < 0) { finish = item.InnerText.ToString().IndexOf(" [", 0); }
				else { finish = item.InnerText.ToString().IndexOf("\t\t", start); }
				int lenght = finish - start;
				if (lenght > 0)
				{
					try { Titles.Add(item.InnerText.ToString().Substring(start + 2, lenght - 2)); }
					catch { Titles.Add(item.InnerText); }
				}
				else
				{
					Titles.Add(item.InnerText);
				}
			}
			int i = 0;
			foreach (var item in Reader.DocumentNode.CssSelect("span[class='tags']"))
			{
				// Coding skills 10/10
				switch (i)
				{
					// case 0 --> Parodies
					// case 1 --> Characters
					case 2: // <-- Tags
						foreach (var rawTags in item.CssSelect("a"))
						{
							Tags.Add(rawTags.InnerText);
						}
						break;
						// case 3 --> Artists
						// case 4 --> Groups
						// case 5 --> Languages
						// case 6 --> Categories
				}
				i++;
			}
		}
		static void SearchByCode(ref string URL, ref HtmlWeb Data, ref List<string> Titles, ref List<string> Tags, ref List<string> Codes, bool IsGenerated)
		{
			Random rand = new Random();
			int Code = 0;
			if (IsGenerated == false)
			{
				bool IsThatACode = true;
				do
				{
					Clear();
					BackgroundColor = ConsoleColor.DarkBlue;
					ForegroundColor = ConsoleColor.White;
					WriteLine("[2]-->[ Search by code         ]\n".ToUpper());
					ResetColor();
					CursorVisible = true;
					Write(">> Insert your code here: ".ToUpper());
					try { Code = int.Parse(ReadLine()); IsThatACode = true; }
					catch (FormatException e)
					{
						ForegroundColor = ConsoleColor.Red;
						CursorVisible = false;
						WriteLine($"[\\Error]: {e.Message}".ToUpper());
						IsThatACode = false;
						ForegroundColor = ConsoleColor.Green;
						Write($"\n[\\PS]: Enter --> 0 to return main menu".ToUpper());
						ReadKey();
					}
				}
				while (Code < 0 || IsThatACode == false);
				if (Code > 0)
				{
					CursorVisible = false;
					ForegroundColor = ConsoleColor.Yellow;
					WriteLine($"\n>> Connecting to nHentai.net".ToUpper());
					Codes.Add("/g/" + Code.ToString() + "/");
				}
			}
			else
			{
				CursorVisible = false;
				BackgroundColor = ConsoleColor.DarkBlue;
				ForegroundColor = ConsoleColor.White;
				WriteLine("[3]-->[ Randomize code search  ]\n\n".ToUpper());
				ResetColor();
				ForegroundColor = ConsoleColor.Yellow;
				WriteLine($">> Generating random code and getting data...".ToUpper());
				Code = rand.Next(1, 315208);
				Codes.Add("/g/" + Code.ToString() + "/");
			}
			if (Code != 0)
			{
				string Thing = $"/g/{Code}";
				URL = "https://www.nhentai.net" + Thing;
				HtmlDocument Reader = Data.Load(URL);
				GetTitleInfo(ref Reader, ref Tags, ref Titles);
				ForegroundColor = ConsoleColor.Green;
				WriteLine(">> Data obtained successfully!".ToUpper());
				ReadKey();
				Clear();
				int respuesta = 0;
				URL = URL.Substring(0, URL.Length - Thing.Length);
				Selected_H(ref respuesta, ref URL, ref Reader, ref Data, ref Titles, ref Codes, ref Tags);
			}
			ReadKey();
			Codes.Clear(); Tags.Clear(); Titles.Clear();
		}
		static void Downloader(ref HtmlDocument Reader, ref HtmlWeb Data, string URL, string SecretCode)
		{
			int UserDownload = 0;
			ForegroundColor = ConsoleColor.Yellow;
			Write($"\n>> Sir, wanna download --> [{SecretCode}] ?".ToUpper());
			ForegroundColor = ConsoleColor.White;
			Write("\n[1]-->[ Yes, please dowload it ]".ToUpper() +
				  "\n[X]-->[ Nah, just watching...  ]".ToUpper() +
				  "\n[Select option]--> ".ToUpper());
			CursorVisible = true;
			try { UserDownload = int.Parse(ReadLine()); }
			catch { ForegroundColor = ConsoleColor.Cyan; CursorVisible = false; Write("\n>> Okay, maybe next time".ToUpper()); }
			if (UserDownload == 1)
			{
				CursorVisible = false;
				string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (!Directory.Exists(Path + @"\Homeworks"))
				{
					Directory.CreateDirectory(Path + @"\Homeworks");
				}
				Directory.CreateDirectory(Path += @"\Homeworks" + $@"\{SecretCode}\");
				ForegroundColor = ConsoleColor.Yellow;
				Write("\n>> Connecting to server...".ToUpper());
				Reader = Data.Load(URL);
				// Get number of pages
				var Number = Reader.DocumentNode.CssSelect(".num-pages").First().InnerText;
				int LastPage = int.Parse(Number);
				// Get image number
				var item = Reader.DocumentNode.CssSelect(".fit-horizontal").First().OuterHtml;
				int temp = item.IndexOf("/galleries/", 0), temp2 = item.IndexOf("/1.jpg", temp);
				if (temp2 <= 0) { temp2 = item.IndexOf("/1.png", temp); }
				string ImageCode = item.Substring(temp + 11, (temp2 - temp) - 11);
				ForegroundColor = ConsoleColor.Green;
				WriteLine("\t[ok]".ToUpper());
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				ForegroundColor = ConsoleColor.White;
				try
				{
					Write(">> Downloading...".ToUpper());
					using (WebClient downloader = new WebClient())
					{
						for (int i = 1; i <= LastPage; i++)
						{
							try
							{
								string URL_Picture = "https://i.nhentai.net/galleries/" + ImageCode + $"/{i}.jpg";
								downloader.DownloadFile(URL_Picture, Path + i + ".jpg");
							}
							catch
							{
								string URL_Picture = "https://i.nhentai.net/galleries/" + ImageCode + $"/{i}.png";
								downloader.DownloadFile(URL_Picture, Path + i + ".png");
							}
						}
						ForegroundColor = ConsoleColor.Green;
						WriteLine($"\t\t[ok]\n".ToUpper());
					}
					ForegroundColor = ConsoleColor.Yellow;
					WriteLine($">> Downloaded in --> [{Path}]".ToUpper());
				}
				catch
				{
					ForegroundColor = ConsoleColor.Red;
					WriteLine(">> Failed to download data :(".ToUpper());
					Directory.Delete(Path, true);
				}
			}
			else { ForegroundColor = ConsoleColor.Cyan; CursorVisible = false; Write("\n>> Press any key to return".ToUpper()); }
		}
	}
}