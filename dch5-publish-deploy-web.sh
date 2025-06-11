# Start ssh-agent and add key
eval $(ssh-agent -s)
ssh-add /c/.ssh/skipper_ed25519

# Restore and Publish
dotnet publish SkipperWeb/SkipperWeb -c Release -f net9.0 -o SkipperWeb/publish -r linux-x64 -p:Platform="Any CPU" --verbosity normal

# Compress published files
tar -czf skipper-web.tar.gz -C SkipperWeb/publish .

# Deploy
REMOTE="skipper@dch5.snailbird.net"
APPROOT="/skipper/web"

ssh $REMOTE "rm -rf $APPROOT/bin/*"
scp skipper-web.tar.gz $REMOTE:$APPROOT/skipper-web.tar.gz
ssh $REMOTE "cd $APPROOT && tar -xzf skipper-web.tar.gz -C bin && rm skipper-web.tar.gz"

# Apply Local Environment
ssh $REMOTE "cp $APPROOT/environment/* $APPROOT/bin/environment"

# Restart the service
ssh $REMOTE "cd $APPROOT && ./restart-web.sh"

# Clean up
rm -rf ./SkipperWeb/publish
rm -f skipper-web.tar.gz
ssh-agent -k

