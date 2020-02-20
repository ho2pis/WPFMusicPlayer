﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using WPFMusicplayer.Commands;

namespace WPFMusicplayer
{
    public class ViewModel : INotifyPropertyChanged
	{
		MusicPlayer musicPlayer;

		public ViewModel()
		{
			musicPlayer = MusicPlayer.Instance;
			StartPauseCommand = new Command(StartPauseExecuteMethod, StartPauseCanExecuteMethod);
			OpenFolderCommand = new Command(OpenFolderExecuteMethod, OpenFolderCanExecuteMethod);
		}


		#region properties

		
		ObservableCollection<MusicElement> _playList = new ObservableCollection<MusicElement>();

		public ICommand StartPauseCommand { get; set; }

		public ICommand OpenFolderCommand { get; set; }

		public ObservableCollection<MusicElement> PlayList
		{
			get { return _playList; }
		}					

		MusicElement _selectedItem;
		public MusicElement SelectedItem
		{
			get { return _selectedItem; }
			set 
			{
				_selectedItem = value;
				musicPlayer.IntiFile(_selectedItem);
			}
		}


		string _searchLabel;
		public string SearchLabel
		{

			get { return _searchLabel; }
			set
			{
				_searchLabel = value;
				OnPropertyChanged("SearchLabel");
				MakePlayList(_searchLabel);
			}
		}
		

		public event PropertyChangedEventHandler PropertyChanged;
		string _filePath;
		public string FilePath
		{
			get { return _filePath; }
			set
			{
				_filePath = value;
				OnPropertyChanged("FilePath");
			}
		}
        #endregion

        #region Commands

		private void StartPauseExecuteMethod(object path)
		{
			if (musicPlayer.IsPlaying)
			{
				musicPlayer.Pause();
				return;
			}
			musicPlayer.Play();
		}

		private bool StartPauseCanExecuteMethod(object obj)
		{

			return true;
			
		}

		private void OpenFolderExecuteMethod(object path)
		{
			//STOP Stream
			musicPlayer.Stop();

			CommonOpenFileDialog cofd = new CommonOpenFileDialog
			{
				IsFolderPicker = true
			};
			if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
			{
				FilePath = cofd.FileName;
				MakePlayList(_searchLabel);
			}
		}

		private bool OpenFolderCanExecuteMethod(object path)
		{
			//change this if any condition added
			return true;
		}

		#endregion
		private void MakePlayList(string keyWord)
		{
			_playList.Clear();
			DirectoryInfo folder = new DirectoryInfo(FilePath);// error
			FileInfo[] files = folder.GetFiles("*" + keyWord + "*.mp3", 0);
			foreach (FileInfo file in files)
			{
				Console.Write(file.FullName);
				_playList.Add(new MP3Element(file.FullName));
			}
		}

		private void AddPath_Click(object sender, RoutedEventArgs e)
		{

		}

		//Inform UI to Update 
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}


}
