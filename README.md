# GitLabCLI
[![Build status](https://ci.appveyor.com/api/projects/status/h240b0xlwssirw3t?svg=true)](https://ci.appveyor.com/project/nmklotas/gitlabcli)
[![codecov](https://codecov.io/gh/nmklotas/GitLabCLI/branch/master/graph/badge.svg)](https://codecov.io/gh/nmklotas/GitLabCLI)

What is GitLabCLI ?  
* It's a cross platform GitLab command line tool to quickly & naturally perform frequent tasks on GitLab project.  
* It does not force you to hand craft json or use other unnatural ways (for example ids, concatenating of strings) like other CLI's to interact with GitLab.  
* It does not have any dependencies.  
* It's self contained .NET core application - you don't need to have .NET installed for it to work.  

## Quick start

### 1. Configure how to authenticate with GitLab API:

To authenticate using username & password:
```
gitlab config --host "https://gitlab-host.com" --username "your username" --password "your password"
```
To authenticate using token:
```
gitlab config --host "https://gitlab-host.com" --token "your token"
```

To boost your productivity it's recommended to set default project also. So you will not need to specify it everytime.
```
gitlab config --default-project "defaultproject"
```

More configuration options can be found [here](https://github.com/nmklotas/GitLabCLI/wiki/Configuration)

### 2. If you want to find more information about certain commands just use --help:

For example:
```
gitlab --help
gitlab config --help
gitlab issue create --help
```

or see Wiki:  
[Creating and listing issues](https://github.com/nmklotas/GitLabCLI/wiki/Creating-and-listing-issues)  
[Creating and listing merge requests](https://github.com/nmklotas/GitLabCLI/wiki/Creating-and-listing-merge-requests)  

## Some examples:

All commands have both short & long syntax.

### Issues
```
# create issue:
gitlab issue create -t "Issue title" -d "Issue description" -l label1,label2

# or long syntax:
gitlab issue create --title "Issue title" --description "Issue description" --labels label1,label2

# create issue for specific user:
gitlab issue create -t "Issue title" -a User

# create issue for yourself:
gitlab issue create -t "Issue title --assign-myself

# create issue in differentproject (not default):
gitlab issue create -t "Issue title" -p differentproject

# close issue 101:
gitlab issue close --id 101

# list issues assigned to me:
gitlab issue list --assigned-to-me

# list issues assigned to User & filtered by label:
gitlab issue list --assignee User -l label1

# list issues having bug in title or description:
gitlab issue list --filter bug

# list issues with ids 101 and 202:
gitlab issue list --id 101,202

# open issue 101 with the default browser:
gitlab issue browse --id 101
```

### Merge requests
```
# create merge request feature -> develop:
gitlab merge create -t "Merge request title" -s feature -d develop

# create merge request feature -> develop with assignee User:
gitlab merge create -t "Merge request title" -s feature -d develop -a User

# list merge requests:
gitlab merge list # list opened
gitlab merge list opened # list opened
gitlab merge list merged # list merged
gitlab merge list closed # list closed
gitlab merge list all # list all

# list merge requests for assignees:
gitlab merge list -a "User" # assigned to user
gitlab merge list --assigned-to-me
```
