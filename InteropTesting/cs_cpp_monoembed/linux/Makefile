all: run

build: client.cpp
	g++ *.cpp -Wno-write-strings -o client.exe `pkg-config --cflags --libs mono-2`

run: build
	./client.exe

dll: YahooAPI.cs
	mcs /target:library YahooAPI.cs


clean:
	rm -f YahooAPI.dll client.exe
