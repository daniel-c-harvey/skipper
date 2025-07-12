# Start ssh-agent and add key
eval $(ssh-agent -s)
ssh-add /c/.ssh/skipper_ed25519

# Generate migration script
echo "Generating migration script..."
dotnet ef migrations script --project SkipperData --context SkipperContext --output skipperdata-migrations.sql --idempotent

# Restore and Publish
dotnet publish SkipperAPI -c Release -f net9.0 -o SkipperAPI/publish -r linux-x64 -p:Platform="Any CPU" --verbosity normal

# Compress published files
tar -czf skipper-api.tar.gz -C SkipperAPI/publish .

# Deploy
REMOTE="skipper@dch5.snailbird.net"
APPROOT="/skipper/api/skipper"

ssh $REMOTE "rm -rf $APPROOT/bin/*"
scp skipper-api.tar.gz $REMOTE:$APPROOT/skipper-api.tar.gz
scp skipperdata-migrations.sql $REMOTE:$APPROOT/skipperdata-migrations.sql
ssh $REMOTE "cd $APPROOT && tar -xzf skipper-api.tar.gz -C bin && rm skipper-api.tar.gz"

# Apply Local Environment
ssh $REMOTE "cp $APPROOT/environment/* $APPROOT/bin/environment"

# Apply database migrations on server
echo "Applying database migrations on server..."
ssh $REMOTE "cd $APPROOT && psql -U skipper -d skipper-test -f skipperdata-migrations.sql && rm skipperdata-migrations.sql"

# Restart the service
ssh $REMOTE "cd $APPROOT && ./restart-api.sh"

# Clean up
rm -rf ./SkipperAPI/publish
rm -f skipper-api.tar.gz
rm -f skipperdata-migrations.sql
ssh-agent -k

