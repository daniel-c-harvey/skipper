# Start ssh-agent and add key
eval $(ssh-agent -s)
ssh-add /c/.ssh/skipper_ed25519

# Deploy
REMOTE="skipper@dch5.snailbird.net"
APPROOT="/skipper/"

scp test.json $REMOTE:$APPROOT/test.json 

# Clean up
ssh-agent -k

