# Start ssh-agent and add key
eval $(ssh-agent -s)
ssh-add /c/.ssh/skipper_ed25519

# Restore and Publish
dotnet publish SkipperPublic/SkipperPublic -c Release -f net9.0 -o SkipperPublic/publish -r linux-x64 -p:Platform="Any CPU" --verbosity normal

# Compress published files
tar -czf skipper-public.tar.gz -C SkipperPublic/publish .

# Deploy
REMOTE="skipper@dch5.snailbird.net"
APPROOT="/skipper/public"

ssh $REMOTE "rm -rf $APPROOT/bin/*"
scp skipper-public.tar.gz $REMOTE:$APPROOT/skipper-public.tar.gz
ssh $REMOTE "cd $APPROOT && tar -xzf skipper-public.tar.gz -C bin && rm skipper-public.tar.gz"


# Restart the service
ssh $REMOTE "cd $APPROOT && ./restart-public.sh"

# Clean up
rm -rf ./SkipperPublic/publish
rm -f skipper-public.tar.gz
ssh-agent -k

