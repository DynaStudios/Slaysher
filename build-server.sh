cd SlaysherServer
rm -Rf bin/
xbuild SlaysherServer.csproj

cd ..
cd SlaysherServerApplication
rm -Rf bin/
xbuild SlaysherServerApplication.csproj
