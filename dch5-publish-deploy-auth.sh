# Start ssh-agent and add key
eval $(ssh-agent -s)
ssh-add /c/.ssh/skipper_ed25519

# Generate migration script
echo "Generating migration script..."
dotnet ef migrations script --project AuthBlocksData --context AuthDbContext --output auth-migrations.sql --idempotent

# Restore and Publish
dotnet publish AuthBlocksAPI -c Release -f net9.0 -o AuthBlocksAPI/publish -r linux-x64 -p:Platform="Any CPU" --verbosity normal

# Compress published files
tar -czf auth-api.tar.gz -C AuthBlocksAPI/publish .

# Deploy
REMOTE="skipper@dch5.snailbird.net"
APPROOT="/skipper/api/auth"

ssh $REMOTE "rm -rf $APPROOT/bin/*"
scp auth-api.tar.gz $REMOTE:$APPROOT/auth-api.tar.gz
scp auth-migrations.sql $REMOTE:$APPROOT/auth-migrations.sql
ssh $REMOTE "cd $APPROOT && tar -xzf auth-api.tar.gz -C bin && rm auth-api.tar.gz"

# Apply Local Environment
ssh $REMOTE "cp $APPROOT/environment/* $APPROOT/bin/environment"

# Apply database migrations on server
echo "Applying database migrations on server..."
ssh $REMOTE "cd $APPROOT && psql -U skipper -d skipper-test -f auth-migrations.sql && rm auth-migrations.sql"

# Restart the service
ssh $REMOTE "cd $APPROOT && ./restart-api.sh"

# Clean up
rm -rf ./AuthBlocksAPI/publish
rm -f auth-api.tar.gz
rm -f auth-migrations.sql
ssh-agent -k

