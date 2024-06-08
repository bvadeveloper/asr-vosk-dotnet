### Install Python on Debian based Linux.
```
sudo apt update
sudo apt install python3
python3 --version
sudo apt install python3-pip
pip3 --version
```

### Install Python dependencies for punctuation '[recasepunc](https://github.com/benob/recasepunc)' model.
Create a virtual environment in any folder where you want to store the installed dependencies.
```
python3 -m venv env
. env/bin/activate
```
Install dependencies (be careful with versions, you can use these [requirements.freeze.txt](https://github.com/benob/recasepunc/blob/main/requirements.freeze.txt) as an example).
```
pip3 install regex==2023.12.25
pip3 install tqdm==4.66.2
pip3 install torch==2.2.2
pip3 install numpy==1.26.4
pip3 install transformers==4.25.1 // fix for russian model: https://github.com/alphacep/vosk-api/issues/1459
```
If you encounter warning or errors like 'rust compiler not found'
try to install it from the official [document](https://www.rust-lang.org/tools/install).

### Download Russian pre-trained models for punctuation.
```
wget https://alphacephei.com/vosk/models/vosk-recasepunc-ru-0.22.zip
unzip vosk-recasepunc-ru-0.22.zip
```

### Example of running command for text punctuation.
Navigate to the extracted folder from a previous step and run command (under virtual environment):
```
python3 recasepunc.py predict checkpoint < ru-test.txt > output.txt
```

### Install ffmpeg
```
sudo apt update
sudo apt-get install ffmpeg
```

### Download a noise suppression model from this [repository](https://github.com/GregorR/rnnoise-models/blob/master/README.md)
```
wget https://github.com/GregorR/rnnoise-models/blob/master/marathon-prescription-2018-08-29/mp.rnnn 
```

### Convert mp4 to wav file with sample rate 16kHz and 256kbps:
To parameter _**arnndn=m=**_ set the correct path to the model file **_mp.rnnn_** from the previous step.
```
// Example: remove the silence from the beginning.
ffmpeg -y -i input_video.mp4 -loglevel quiet -vn -acodec pcm_s16le -ar 16000 -ac 1 -af silenceremove=1:0:-50dB:detection=peak output_audio_256kbps_16khz_mono.wav

// Example: remove the silence from the beginning + speech normalizer + noise suppression.
ffmpeg -y -i input_video.mp4 -loglevel quiet -vn -acodec pcm_s16le -ar 16000 -ac 1 -af silenceremove=1:0:-50dB:detection=peak,speechnorm=e=3:r=0.00001:l=1,arnndn=m=/data/mp.rnnn output_audio_256kbps_16khz_mono.wav

Params:
    -acodec pcm_s16le : sets the audio codec to PCM signed 16-bit little-endian, which is a common format for WAV files.
    -ar 16000 : sets the audio sample rate to 16.0 kHz, which is a standard sample rate for speech recognition.
    -ac 1 : specifies that the output audio should have one channel (mono).
    -af silenceremove=1:0:-50dB:detection=peak : remove silence in the output file.
    -af arnndn=m=/media/data/prototypes/ConsoleAppSpeechRecognition/mp.rnnn : add noise suppression filter.
```

### Download full Russian pre-trained models for speech recognition from [alphacephei.com](https://alphacephei.com/vosk/models)
```
wget https://alphacephei.com/vosk/models/vosk-model-ru-0.42.zip
unzip vosk-model-ru-0.42.zip
```

### Set the correct paths to models and wav files in Program.cs (extract data.zip)
```
// wav files:
private const string InputWavFileNameShort = "/data/test_256kbps_16000hz_mono.wav";
private const string InputWavFileNameLong = "/data/output_audio_256kbps_16khz_mono.wav";

// model dir:
private const string ModelPath = "/data/model_full";
```

### Example:
```
о чем шумите вы
народные витии зачем анафемой грозите вы россии что возмутило вас во мнении литвы оставьте это спор славян между собой домашний давний спор уж взвешенный судьбою вопрос которого не разрешите вы уже давно между собою враждуют эти
и раз склонились под грозою то их то наша сторона кто устоит в неравном споре кичливый лях или верный росс славянские ручьи сольются в русском море
ноль иссякнет вот вопрос оставьте нас вы не читали сии кровавые скрижали вам непонятна вам чужда сия семейная вражда для вас безмолвны кремль и прага бессмысленно прельщает вас борьбы отчаянной отвага и ненавидите война за что ж от
ответствуйте за то ли что на развалинах пылающей москвы мы не признали наглой воли того под кем дрожали вы
за то ли что в бездну повалили мы тяготеющий на царственный кумир и нашей кровью искупили европы вольность честь и мир вы грозны на словах попробуйте на деятелей или старый богатырь покойный постели не в силах завинтить свой измаильский штык или русского царя уже бессильно слово и нам
европа и спорить ново или русский от побед отвык или мало нас или от перми до тавриды от финских хладных скал до пламенной колхиды от потрясённой кремля до стен недвижного китая стальной щетиною сверканья и встанет русская земля так высылайте ж к нам людей своих ослов
верных сынов есть место и в полях россии
среди чуждых им
гробов
```

### Example with punctuation:
```
О чем шумите вы ?
Народные витии Зачем анафемой грозите вы, россии что возмутило вас во мнении литвы Оставьте это спор славян между собой. Домашний давний спор, уж взвешенный судьбою вопрос, которого не разрешите Вы уже давно между собою враждуют эти.
И раз склонились под грозою то их, то наша сторона. Кто устоит в неравном споре кичливый лях или верный росс. Славянские ручьи сольются в русском море.
Ноль иссякнет Вот вопрос. Оставьте нас. Вы не читали сии кровавые скрижали вам непонятна Вам чужда сия семейная вражда. Для вас безмолвны Кремль и Прага бессмысленно прельщает вас, борьбы отчаянной отвага и ненавидите война за что ж от ?
Ответствуйте за то ли, что на развалинах пылающей москвы мы не признали наглой воли того, под кем дрожали вы.
За то ли, что в бездну повалили мы, тяготеющий на царственный кумир и нашей кровью искупили европы вольность честь и мир. Вы грозны на словах попробуйте на деятелей или старый богатырь покойный постели, не в силах завинтить свой измаильский штык или русского царя уже бессильно слово и нам.
Европа и спорить ново или русский от побед отвык или мало нас, или от перми до тавриды от финских хладных скал до пламенной колхиды от потрясённой Кремля до стен недвижного китая стальной щетиною сверканья и встанет русская земля. Так высылайте ж к нам людей своих ослов
Верных сынов есть место и в полях россии
Среди чуждых им.
Гробов
```