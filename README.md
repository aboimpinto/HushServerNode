# For developers

This project uses *submodules* that will became NuGet packages one day. 

In order to clone the project and get the submodules it's possible to do in two ways: 

```
    $ git clone --recursive-submodule git@github.com:aboimpinto/HushServerNode.git
```

or 

```
    $ git clone git@github.com:aboimpinto/HushServerNode.git
    $ git submodule init
    $ git submodule update
```

# Remove a submodule

According with this StackOverflow: https://stackoverflow.com/questions/1260748/how-do-i-remove-a-submodule

```
    $ git rm <path-to-submodule>
```

or follow the guide here: https://gist.github.com/myusuf3/7f645819ded92bda6677

# Basic arquitecture
![image](https://github.com/aboimpinto/HushServerNode/assets/1231687/5355fcfc-f2c9-4f69-a58f-fde87e1ba0f3)

