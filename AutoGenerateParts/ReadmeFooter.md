

# Post-Cloning Setup on Windows

This repository utilizes symbolic links to share code across multiple projects.
On Windows 10 or later, symbolic link creation is restricted by default for regular users,
which can lead to issues when working with this repository.

To resolve this, follow the steps below to enable symbolic link creation and configure Git appropriately.

1. Open **Local Security Policy** by typing `secpol.msc` on Windows Command Prompt

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_LocalSecurityPolicy.jpg)


2. Under **User Rights Assignment**, find a policy called **Create symbolic links** and open it.
  - Click **Add User or Group**
  - Click **Object Types**
  - Make sure **Groups** is checked and click **OK**.

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_Properties.jpg)

3. Type **USERS** inside the textbox and click on **Check Names** to verify it, then click **OK**.

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_SelectUsers.jpg)

4. Configure git to allow symbolic links. For example, by typing the following in Git Bash:

```
git config --local core.symlinks true
git config --global core.symlinks true
```

## License
* Source Code: [Unity Companion License](com.unity.toonshader/LICENSE.md)
* Unity-chan Assets: [Unity-Chan License](http://unity-chan.com/contents/guideline_en/)  
  These assets can be located under, but not limited to, the following folder:
    - `com.unity.toonshader/Samples~`
