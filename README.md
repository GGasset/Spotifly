# Spotifly
#### Video Demo:  https://youtu.be/bEz0eRnQk-c
#### Description:
Spotifly is a C# winforms app whose primary functions are a Media player and an in-site Youtube downloader.
To complement that it has a folder browser which filters the files to include only media files, more features later.
There is also a media queue, you can add items to it and they will be played in the order that you have specified, about more features later.
To let the user expand its media library, there is a page that lets users browse youtube, watch videos directly on youtube and download any video of any length as long as you have enough storage space.
There is also a settings page, about all its features later.

To start diving deep lets see Form1.cs:
There are some declared variables like an array of panels so they can be formatted to display to users and manipulated easier, a hash table to store plain text, settings and more!
Form1 constructor:
Then there is Form1 constructor where the Theme is changed a default directory is created if it doesn't exists, panels position and size is declared.

Form1_Load():
Panel[] is initialized and manually filled.
hash table is also filled with all the necessary plain text.
A PowerModeChange event is linked with the respective function so that when your pc sleeps the media is paused and a Stop Watch is initialized so when the function is almost finished a minWait of 600ms is waited.
Then panels are moved and resized, folder browser is filled with media files and all the settings and previous session info is applied.
Finally, update timer is started, more about that later.

Form1_FormClosing():
All settings get saved.

SetActivePanel(int panelIndex):
A foreach loop starts and loop panel.Visible = current loop panel ReferenceEquals(panels[panelIndex])
Then BrowserBackButton and DownloadGroup .Visible fields are set whether panel index is the panel index of the browser page and whether if it isn't in homepage and if its in a video respectively.

SetFormSizeForCurrentMedia():
If user's settings are set to use this function, the function resizes the form so the media fills the whole media player given a minimum size

The rest of the file are page-changing buttons and MouseEnter / MouseLeave functions that change the color of a particular element on the form.

Controls.cs:
This file defines events, functionalities and data processing functions
There are this functions:
* PlayButton_Click()
* ProgressBar_MouseClick() // Moves the video played point around based on where the user clicked
* PrevMediaButton_Click() // Goes to the previous media on the playlist
* NextMediaButton_Click()
* ShuffleButton_Click() // calls the SetShuffleButton(!shuffle)
* SetShuffleButton(bool value):
    - If (shuffle = value)
    {
        back color rgb is shifted a fourth of the difference to a mid-value rgb value
    }

* ShufflePlaylist(string[] playlist)
* FullScreenButton_Click()
* VolumeTrackBar_Scroll()
* ElapsedTimeLabel_MouseClick()
* GetRemainingTimeString()
* Brush progressBarBrush = Brushes.White;
* SetProgressBarValueForCurrentMediaPos()

Media Settings Form:
A form to tell the app when and how to manipulate files

MediaSettingsForm.cs:
* MediaSettingsForm(Form1 principalForm) constructor:
    - options are declared in a list of strings
    {
        Create folder,
        Delete item,
        Rename item,
        Copy item,
        Move item,
        Copy item to base folder,
        Move item to base folder
    };

    - options are added to a combo box in the form through a foreach loop and a false value is added to another list of boolean values representing that there isn't any selected options is checked as in-use.
* SetMediaOptionsCheckBox()
    // This sets if the selected option is in use
* MediaOptionsCheckBox_CheckedChanged()
    - If (!isSelected)
    {
        SetInputMode(false);
    }
    else if (GetSelectedOption == Create folder)
    {
        string folderName = PrincipalForm.folderPath;
        SetInputMode(true, PrincipalForm.URLToName(folderName));
    }
    - ChangeCurrentOption(MediaComboBox.Text);
    - optionPath = string.Empty;
* MediaOptionsComboBox_TextChanged()
    - if textBoxMode
    {
        SetInputMode(false);
    }
    SetMediaOptionsCheckBox(false);
    optionPath is cleared;
* SetInputMode(bool v, string textToModify)
    - if (v)
    {
        this.Size = RenamingSize;
    }
    else
    {
        this.Size = StartingSize;
        textToModify = string.Empty;
    }

    - textBoxMode and its related components .Visible field are set to v;
    - PathToModifyLabel.Text = textToModify;
    - ItemTextBox.Text = string.Empty;
* SetItemTextBoxText(string text)
* ChangeCurrentOption(string option)
    // Options boolean list gets one value set to true if the checkBox is checked
* GetSelectedOption()
    - if (checkBox isn't checked)
    {
        return "None";
    }
    - return ComboBox.Text;
* ConfirmRenameButton_Click()
* CreateFolder()
* RenameFile()
* FileRenameTextBox_TextChanged()
    // This deletes System illegal characters

DownloadedMedia.cs:
This file mostly declares functionality and adds some events
* Declared fields:
    - mediaSettingsForm
    - bool addToQueue
    - string initialFolderPath
    - string[] filteredFilesMemory, foldersMemory
    - string fileFilterMemory
    - string[] supportedExtensions
* MediaListView_ItemSelectionChanged()
    - string pathWithoutExtensions
    - bool isDirectory
    // Do things depending on if mediaSettings form is disposed or else depending on the option selected option
* HandleMediaListViewItemClickWhenNoOptionSelected(string itemText, string pathWithoutExtensions, bool isDirectory, string folderPath)
* AddToQueue(string folderName)
* ChangeDirectory(string folderName)
* ChangeMedia(string mediaName)
* DeleteItem(string itemName, string pathWithoutExtensions, bool isDirectory)
* GetFullPathForFile(string folderPath, string path)
* MediaListView_DrawMedia(string fileFilter = null, bool unshuffleNeed = false)
* GetFilteredFilesAndFolders(string path, out string[] files, out string[] folders)
* FilterFiles(string[] files)
* BackupInMemory(folders, filteredFiles)
* AppendArrays(ar1, ar2)
* ArrayElementsEqual(ar0, ar1)
* SetListViewItems(files, folders, fileFilter = "")
* FilterFilesByFilter(files, filter)
* SearchTxtBox_TextChanged()
* BackBttn_Click()
* OpenCurrentFolder_Click()
* ClearFileFilter_Click()
* GetFolderFromURL(mediaURL)


If you enter "Browser" page you will see Youtube open up, after you agree their terms,
you may enter any Youtube video inside playlist or not and a box will appear, there are 2 buttons on the box that declares a Download Status label and the status itself,
one button is used to download the media in a video file with a max quality of 1080 x 720p whose text is "Video" and the other 

---

#### To install:

##### - right click on Solution > Restore NuGet packages

##### - At just below the top of the Visual Studio window click start and enjoy!
---
#### If you still can't build because there are errors:

##### - right click on Solution > Restore NuGet packages
##### - right click on Solution > Clean Solution
##### - right click on Solution > Build Solution
##### - Close Visual Studio and re-open.
##### - Rebuild solution. 
If these steps don't resolve your issue try repeating the steps a second time.
---
![Main-Page](https://github.com/GGasset/Spotifly/blob/main/Images/Main%20page.png?raw=true)
![Downloaded-Media-List](https://github.com/GGasset/Spotifly/blob/main/Images/Downloaded%20media.png?raw=true)
![Youtube-Browser](https://github.com/GGasset/Spotifly/blob/main/Images/Youtube%20Browser.png?raw=true)
