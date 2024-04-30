
![Ethereal Cloud Logo](Ethereal%20Cloud/wwwroot/images/LogoVector.svg)

# Ethereal Cloud





Ethereal Cloud is a cloud storage solutions in which users can upload, share and download files from anywhere and on any device.

As a group that believes in accessibility and ease of use for our users. Our vision for this project was to allow everyone to quickly and efficiently access, store and share files with a user friendly interface. We achieved this by hosting our cloud storage on a web application which gives it the capacity to run on any device. With the global shift towards online technology and the internet becoming a crucial
part of the everyday world for both companies and individuals, as a team we wanted to
tap into the growing demand for secure cloud storage solutions. Our goal was to make this product accessible, affordable, and useful for all of our users.
## How it works

There are 4 major parts to this project: The Website + WebAPI, the Storage Controller, the Database and the Buckets. The purpose of the website + webAPI is to both serve pages to users on the internet and to provide a bridge between the website and the storage controller. The storage controller's job is to interact with the database in order to save and retrieve user, file and folder data to enable a lot of the functionality of the website, the storage controller also handles storing the file content in buckets, a system that can be scaled up and adjusted to allow load balancing, splitting storage between buckets and adding redundant file storage as backups.

The solution uses docker to help solve the age old problem of "it works on my machine!" and so that no matter the software setup, the different parts of the solution can run, given the hardware is strong enough to handle it.
## Features

- **Cross platform -** *Our web application is accessible on any device with access to an internet connection.*

- **Create account -** *Users can create an account in which they can use to login anywhere.*

- **Change passwords -** *The user can change the password of their account.*

- **2 factor authenticator -** *When the users logs in to their account an email is sent containing a 6 digit code. This code can then be inputted to gain access to the website.*

- **Upload files -** *All files types can be uploaded where they will be stored on the docker volume.*

- **Share files -** *The users files can be shared with any user by knowing their username.*

- **Download files -** *Files which have been uploaded or shared with the user can be downloaded onto the users device.*

- **View files your sharing -** *The user can view the files they are sharing and which users they are sharing the files with.*

- **Unsharing files -** *Shared files can be stopped from sharing with specific users.*

- **View files shared with you -** *Users can see files which have been shared with them.*

- **Create folders -** *Folders can be created and named in which files can be uploaded into.*

- **Delete files / folders -** *Folders and files can both be deleted and will be moved into the bin.*

- **Restore files -** *Files that have been deleted can be restored from the bin.*

- **Sorting -** *The files and folders in the upload, bin and sharing can be sorted in alphabetical or reverse alphabetical.*

- **Renaming files / folders -** *The users can rename their files and folders and this is updated for any files which are being shared.*

## Demo

[![Demo](https://img.youtube.com/vi/us9HXNIlpzI/0.jpg)](https://www.youtube.com/watch?v=us9HXNIlpzI)

## Deployment

To deploy this project open docker and run the batch script.

```bash
build-docker.bat
```

This will create the neccessary docker images.
When complete the localhost below can be used to access the web application.
```
https://localhost:8081/
```


## Authors
- [@Benjamin Sanders-Wyatt](https://github.com/benjaminsanderswyatt)
- [@Eli Bowen](https://github.com/elij35)
- [@Riley Coulstock](https://github.com/aerdjnr)
- [@William Harding](https://github.com/WilliamHarding420)

