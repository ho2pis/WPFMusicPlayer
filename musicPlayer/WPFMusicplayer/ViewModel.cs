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
		public MusicPlayer MusicEngine
		{
			get;
			set;
		}

		public ViewModel()
		{
			MusicEngine = MusicPlayer.Instance;
			audioProgress.Click;
			StartPauseCommand = new DelegateCommand(StartPauseExecuteMethod, ControlRelatedButtonCanExecuteMethod);
			NextFileCommand = new DelegateCommand(NextExecuteMethod, ControlRelatedButtonCanExecuteMethod);
			BeforeFileCommand = new DelegateCommand(BeforeExecuteMethod, ControlRelatedButtonCanExecuteMethod);
			SkipSecNextCommand = new DelegateCommand(SkipSecNextExecuteMethod, ControlRelatedButtonCanExecuteMethod);
			SkipSecBeforeCommand = new DelegateCommand(SkipSecBeforeExecuteMethod, ControlRelatedButtonCanExecuteMethod);
			OpenFolderCommand = new Command(OpenFolderExecuteMethod, OpenFolderCanExecuteMethod);

		}
		

		#region properties

		
		ObservableCollection<MusicElement> _playList = new ObservableCollection<MusicElement>();


		
		public ICommand StartPauseCommand { get; set; }

		public ICommand NextFileCommand { get; set; }

		public ICommand BeforeFileCommand { get; set; }

		public ICommand SkipSecNextCommand { get; set; }

		public ICommand SkipSecBeforeCommand { get; set; }

		public ICommand OpenFolderCommand { get; set; }



		public ObservableCollection<MusicElement> PlayList
		{
			get { return _playList; }
		}					

		private int IndexOfSelectedItem { get; set; }


		private bool IsIndexOfSelectedItemCurrent
		{
			get;set;
		}


		MusicElement _selectedItem;
		public MusicElement SelectedItem
		{
			get { return _selectedItem; }
			set 
			{
				IsIndexOfSelectedItemCurrent = false;
				_selectedItem = value;
				if (_selectedItem != null) MusicEngine.IntiFile(_selectedItem);
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

		//Start/Pause button
		private void StartPauseExecuteMethod(object obj)
		{
			if (MusicEngine.IsPlaying)
			{
				MusicEngine.Pause();
				return;
			}
			MusicEngine.Play();
		}

		private bool ControlRelatedButtonCanExecuteMethod(object obj)
		{
			if (_selectedItem == null)
				return false;
			if (PlayList.Count <= 0)
				return false;
			return true;
			
		}

		//NextFileButton
		private void NextExecuteMethod(object obj)
		{
			if(!IsIndexOfSelectedItemCurrent)
			{
				this.UpdateIndexOfSelectedItem();
			}

			if (IndexOfSelectedItem >= PlayList.Count - 1)
				IndexOfSelectedItem = 0;
			else
				IndexOfSelectedItem++;

			_selectedItem = PlayList[IndexOfSelectedItem];
			MusicEngine.IntiFile(_selectedItem);

		}


		//BeforeFileButton
		private void BeforeExecuteMethod(object obj)
		{
			if (!IsIndexOfSelectedItemCurrent)
			{
				this.UpdateIndexOfSelectedItem();
			}

			if (IndexOfSelectedItem <= 0)
				IndexOfSelectedItem = PlayList.Count - 1;
			else
				IndexOfSelectedItem--;

			_selectedItem = PlayList[IndexOfSelectedItem];
			MusicEngine.IntiFile(_selectedItem);

		}


		//OpenFolder(Path)Button
		private void OpenFolderExecuteMethod(object obj)
		{
			//STOP Stream
			MusicEngine.Stop();

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

		private bool OpenFolderCanExecuteMethod(object obj)
		{
			//change this if any condition added
			return true;
		}

		//SkipSecNextButton
		private void SkipSecNextExecuteMethod(object obj)
		{
			MusicEngine.ActiveStream.Skip(10);
		}


		//SkipSecNextButton
		private void SkipSecBeforeExecuteMethod(object obj)
		{
			MusicEngine.ActiveStream.Skip(-10);
		}


		#endregion

		private	void UpdateIndexOfSelectedItem()
		{
			IndexOfSelectedItem = _playList.IndexOf(_selectedItem);
			IsIndexOfSelectedItemCurrent = true;
		}

		private void MakePlayList(string keyWord)
		{
			_playList.Clear();
			DirectoryInfo folder = new DirectoryInfo(FilePath);// error
			FileInfo[] files = folder.GetFiles("*" + keyWord + "*.mp3", 0);
			foreach (FileInfo file in files)
			{
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
