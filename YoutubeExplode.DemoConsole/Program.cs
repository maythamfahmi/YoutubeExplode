﻿using System;
using System.Threading.Tasks;
using YoutubeExplode.DemoConsole.Internal;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.DemoConsole
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            Console.Title = "YoutubeExplode Demo";

            // This demo prompts for video ID and downloads one media stream
            // It's intended to be very simple and straight to the point
            // For a more complicated example - check out the WPF demo

            var youtube = new YoutubeClient();

            // Read the video ID
            Console.Write("Enter YouTube video ID or URL: ");
            var videoIdOrUrl = Console.ReadLine();

            // Get media streams & choose the best muxed stream
            var streams = await youtube.Videos.Streams.GetManifestAsync(videoIdOrUrl);
            var streamInfo = streams.GetMuxed().WithHighestVideoQuality();
            if (streamInfo == null)
            {
                Console.Error.WriteLine("This videos has no streams");
                return -1;
            }

            // Compose file name, based on metadata
            var fileExtension = streamInfo.Container.GetFileExtension();
            var fileName = $"{videoIdOrUrl}.{fileExtension}";

            // Download video
            Console.Write($"Downloading stream: {streamInfo.VideoQualityLabel} / {fileExtension}... ");
            using (var progress = new InlineProgress())
                await youtube.Videos.Streams.DownloadAsync(streamInfo, fileName, progress);

            Console.WriteLine($"Video saved to '{fileName}'");
            return 0;
        }
    }
}
