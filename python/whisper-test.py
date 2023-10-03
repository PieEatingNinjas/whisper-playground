import requests

AZURE_OPENAI_ENDPOINT = '<<REDACTED>>'
AZURE_OPENAI_KEY = '<<REDACTED>>'
MODEL_NAME = 'whisper'
HEADERS = { "api-key" : AZURE_OPENAI_KEY }

FILE_LIST = {
    "a": "what-is-it-like-to-be-a-crocodile-27706.mp3",
    "b": "toutes-les-femmes-de-ma-vie-164527.mp3",
    "c": "what-can-i-do-for-you-npc-british-male-99751.mp3"
    }

print('Choose a file:')

for i in FILE_LIST:
    print(f'{i} -> {FILE_LIST[i]}')

file = FILE_LIST.get(input())

if file:
    with open(f'../assets/{file}', 'rb') as audio:
        r = requests.post(f'{AZURE_OPENAI_ENDPOINT}/openai/deployments/{MODEL_NAME}/audio/transcriptions?api-version=2023-09-01-preview', 
                        headers=HEADERS, 
                        files={'file': audio})

        print(r.json().get('text'))
else:
    print('invalid file')