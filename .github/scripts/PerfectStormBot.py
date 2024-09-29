#installation guide:
#1.go to: -> https://slack.com/oauth/v2/authorize?client_id=7749399447511.7757324171238&scope=channels:history,channels:read,chat:write&user_scope=
#2.in upper right corner select your workspace and clik allow, you will be redirected to https://httpbin.org/get
#3.from site where you have been redirected copy "code" value it should look something like this: "7779412170229.7782584275715.a18ef81c48d45d4805edac2c11963ccd82fdb41977dcf01bce74c86bdf9f4afc"
#4.run installbot.exe should be placed in shared and give it code
#5.script will output bot token that will look something like this: xoxe.xoxb-1-MS0yLTc3Nzk0MTIxNzAyMjktNzc3OTUzNzc2NDY2MS03NzgyNDA1MzM3MzQ2LTc3ODIzMDk5NzAwMzUtNDVlMGVhNmRhMThlZDkxNmFjZGViY2NjZjRiZmYxZmI5YTVjYzE3NjJiYzVmOWQzZTUzM2Q1MjE5MzFjYzMxYg
#6.copy bot token and create new repo secret "SLACK_BOT_TOKEN" with value of copied token

from slack_sdk import WebClient 
from slack_sdk.errors import SlackApiError 
import sys

message=""
channel_name=""
slack_token=""

#We provide last commit info to script by arguments
if len(sys.argv) > 4:
    channel_name=sys.argv[1]
    slack_token=sys.argv[2]
    branch_name=sys.argv[3]
    commit_url=sys.argv[4]
    hyperlink=f"<{commit_url}|{branch_name}>"
    message=f"Branch {hyperlink} has been merged to main branch\nPlease update your branches"
else:
    sys.exit("not valid arguments use arguments: channel_name bot_token branch_name commit_url")

client = WebClient(token=slack_token)

def get_channel_id(channel_name):
    try:
        response = client.conversations_list()
        for channel in response['channels']:
            if channel['name'] == channel_name:
                return channel['id']
            
    except SlackApiError as e:
        print(f"Error fetching channels: {e.response['error']}")

#slack indetify messages by their timestamps
def get_bot_message_timestamp(channel_id):
    try:
        response = client.conversations_history(channel=channel_id)
        messages = response['messages']
        for message in messages:
            if 'bot_id' in message:
                return message['ts']
            
    except SlackApiError as e:
        print(f"Error fetching messages: {e.response['error']}")

def post_message(channel_id, message_text):
    try:
        client.chat_postMessage(
            channel=channel_id,
            text = message_text
        )
        
    except SlackApiError as e:
        print(f"Error posting message: {e.response['error']}")
    
def delete_message(channel_id, timestamp):
    try:
        client.chat_delete(
            channel=channel_id,
            ts=timestamp
        )
    except SlackApiError as e:
        print(f"Error deleting message: {e.response['error']}")


channel_id = get_channel_id(channel_name)
timestamp = get_bot_message_timestamp(channel_id)

#if its first bot message we cant try to delete previous one
if(timestamp != None):
    delete_message(channel_id,timestamp)

post_message(channel_id,message)