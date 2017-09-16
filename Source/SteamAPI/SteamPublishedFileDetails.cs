using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Creventive.SteamAPI
{
    public class SteamPublishedFileDetails : SteamResponse
    {
        public long PublishedFileId { get; set; }

        [JsonProperty("creator")]
        public string CreatorSteamId { get; set; }

        [JsonProperty("creator_app_id")]
        public long CreatorAppId { get; set; }

        [JsonProperty("consumer_app_id")]
        public long ConsumerAppId { get; set; }

        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("file_url")]
        public Uri FileUrl { get; set; }

        [JsonProperty("hcontent_file")]
        public string HContentFile { get; set; }

        [JsonProperty("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("hcontent_preview")]
        public string HContentPreview { get; set; }

        public string Title { get; set; }

        [JsonProperty("time_created")]
        public long TimeCreated { get; set; }

        public int Visibility { get; set; }

        public int Banned { get; set; }

        [JsonProperty("ban_reason")]
        public string BanReason { get; set; }

        public long Subscriptions { get; set; }

        public long Favorited { get; set; }

        [JsonProperty("lifetime_subscriptions")]
        public long LifetimeSubscriptions { get; set; }

        [JsonProperty("lifetime_favorites")]
        public long LifetimeFavorites { get; set; }

        public long Views { get; set; }

        public List<SteamTag> Tags { get; } = new List<SteamTag>();
        /*{
                "response": {
                    "result": 1,
                    "resultcount": 1,
                    "publishedfiledetails": [
                        {
                            "publishedfileid": "822950976",
                            "result": 1,
                            "creator": "76561197977434775",
                            "creator_app_id": 244850,
                            "consumer_app_id": 244850,
                            "filename": "tmpb610.tmp",
                            "file_size": 31002,
                            "file_url": "http://cloud-3.steamusercontent.com/ugc/172662564415881677/5087E15E0437268EB3EB807EF1FF4066D79BBD0B/",
                            "hcontent_file": "172662564415881677",
                            "preview_url": "http://cloud-3.steamusercontent.com/ugc/172662564415881801/02EA4CE6F6F732C8A0435A0B847C81923E0E8BDB/",
                            "hcontent_preview": "172662564415881801",
                            "title": "Automatic LCDs 2",
                            "description": "[b]Everything you will ever need to know about your ship and station displayed in real time on LCD panels in any vanilla games. modded games and servers![/b]\r\n\r\nv:2.0004 (Update for game v01.172) Fixed LCD linking, LCD_TAG can be set in Custom Data of PB, updated deprecated API stuff [check Change Notes above screenshots!]\r\nIn-game script by MMaster\r\n\r\n\r\n[h1][b]\r\n= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =\r\nScript now uses only Custom Data for commands! LCD Title is ignored.\r\n= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =\r\n[/b][/h1]\r\n\r\n\r\n[h1]NEW! Basics video guide[/h1]\r\nPlease watch the video guide even if you don't understand my English. You can see how things are done there.\r\n\r\nhttps://youtu.be/vqpPQ_20Xso\r\n\r\n\r\n[h1][b]##################################################\r\n READ THIS [i]FULL GUIDE[/i]\r\n http://steamcommunity.com/sharedfiles/filedetails/?id=407158161\r\n##################################################[/b][/h1]\r\n\r\n\r\n[h1][b]Complete rework of script core\r\n - automatically updates as many LCDs as possible as fast as possible!\r\n - can be used on ships of any size! \r\n - never throws \"Script too complex\" exception!\r\n - works optimally with timer block set to Trigger Now\r\n - many new features (check Change Notes above the screenshots)[/b][/h1]\r\n\r\n\r\n[b]Manages multiple LCDs based on commands written in LCD Custom Data.\r\n[list]\r\n[*][h1]Stable & always up-to-date with game![/h1]\r\n[*]Jump drives charge display\r\n[*]Oxygen pressure, farms output & tanks content!\r\n[*]Damaged blocks list with progress bars!\r\n[*]Filtered inventory items listing & missing items listing\r\n[*]Reactor, solar & battery power stats\r\n[*]Block details display\r\n[*]Doors, gears & connectors status\r\n[*]Ammo report\r\n[*]Custom scrolling text display\r\n[*]Linked LCDs!\r\n[*]Works on LCD, Wide LCD, text panels and their modded counterparts!\r\n[*]Any font size!\r\n[*]Filter blocks by name or by group\r\n[*]Cargo space\r\n[*]Block count\r\n[*]Producing, Idle & Enabled summary / list\r\n[*]Laser antenna status\r\n[*]Stopping distance and stopping time\r\n[*]Location, Speed, Acceleration, Date & Time, Gravity\r\n[*]Nicely formatted text with progress bars!\r\n[*]Multiple commands in single LCD!\r\n[*]Works on any server!\r\n[/list]\r\n\r\nNO PROGRAMMING NEEDED.[/b]\r\n\r\n[i]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\nEvery captain wants to have displays that show some useful info. Make your bridge display damaged blocks in engineering, engine room, etc. Make big screen by joining multiple Wide LCDs! Show power output, batteries status, laser antenna connections and much more. Make your docking bay display which landing gears are occupied. Make screens for docking fighers when landing gear is ready to dock so they can nicely see it from cockpit! Make one LCD per container to see its contents.. and much more!\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~[/i]\r\n\r\n[h1]German version (UPDATED)[/h1]\r\nThanks a lot to JD.Horx for translating:\r\nhttp://steamcommunity.com/sharedfiles/filedetails/?id=858003266\r\n\r\n[h1]French version (UPDATED)[/h1]\r\nThanks a lot to =SW42=Hydeki_ for translating: \r\nhttp://steamcommunity.com/sharedfiles/filedetails/?id=822980231 \r\n\r\n[h1]Spanish version (UPDATED)[/h1]\r\nThanks a lot to Makarov for translating:\r\nhttp://steamcommunity.com/sharedfiles/filedetails/?id=698634650\r\n\r\n[h1]Russian version (UPDATED)[/h1]\r\nThanks a lot to Alex2K for translating:\r\nhttp://steamcommunity.com/sharedfiles/filedetails/?id=787082314\r\n\r\n\r\n[h1][b]Most helpful community member[/b][/h1]\r\nHuge thanks to [b]Kham[/b] for helping people in the comments below for very long time. It's very very nice of him!\r\n\r\n\r\n[h1]STEAM GROUP[/h1]\r\nI usually notify about updates on twitter & recently I created Steam group so follow/join if interested.\r\nhttp://steamcommunity.com/groups/mmnews\r\n\r\n[h1]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~[/h1]\r\n[h1][b]CHECK MY OTHER MODS [i](almost all 5 star)[/i][/b][/h1]\r\nhttp://steamcommunity.com/id/mmaster/myworkshopfiles/?browsefilter=myfiles\r\n[h1]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~[/h1]\r\n\r\n\r\n[b]Please DO NOT publish this script or its derivations without my permission! Feel free to use it in blueprints![/b]\r\n\r\nTHE SCRIPT IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SCRIPT OR THE USE OR OTHER DEALINGS IN THE SCRIPT.\r\n\r\n\r\n[h1][b]HOW TO UPDATE?[/b][/h1]\r\nThis script is not a mod so it does not update by itself. You need to load new version of script to your programmable block to overwrite the old one. You do that in exactly the same way as when you first loaded the script into programmable block.\r\nOpen your programmable block, click Edit, click Browse Workshop, select Configurable Automatic LCDs, click OK, Check code, Remember & Exit. Done. Your script is now updated.\r\n\r\n[h1][b]RATING[/b][/h1]\r\nIf you like this script please give it positive rating, it gives me \"fuel\" to continue.\r\nIf you plan to give it negative rating, please at least let me know what makes it so horrible that it deserves negative rating so I can learn from the mistake and/or fix it.\r\n\r\n\r\n[h1][b]!! PLEASE READ THIS !![/b][/h1]\r\nI'm really happy that this script became so popular and that so many people like it, but many people means many questions and I rather enjoy other things than saying the same things all over again to people. So I wrote the guide where I've put everything about every command, tried to answer all questions that people had in comments and I keep it updated. I'm responding to many comments that are obviously stated in the guide. Please please read the guide.\r\n\r\n[b]If you've got questions, but you don't want to read the guide, then this script is probably not for you.[/b]\r\n\r\nI understand that this script got quite a lot of features and it can be hard to crunch whole guide. Unfortunately I can't teach everyone how to use it via chat. [i]It took me several [b]hundreds of hours[/b] to put this all together so please take your time to read the guide and follow the troubleshooting section if something doesn't work.[/i]\r\n\r\nIf you have problem with some command then read the guide section for that command and make sure you use it correctly. Try to use it on separate LCD by itself so it's easier for you to see the issue and definitely try some examples! \r\n\r\nYou are the space engineers so go ahead and play with it and figure it out :)\r\n\r\nThank you very much for your understanding.\r\n\r\n\r\n[h1]TROUBLESHOOTING[/h1]\r\n\r\n[h1][b]\r\n*\r\nALWAYS CHECK OWNERSHIP OF YOUR BLOCKS!\r\n*\r\n[/b][/h1]\r\n\r\nLook at Troubleshooting section in Full guide for possible help. Read the guide again and be extra extra careful about each step, always check your command syntax, try the examples and read the troubleshooting section.\r\n\r\n\r\n[h1]Special Thanks[/h1]\r\nKeen SWH for awesome game Space Engineers\r\nMalware for his work on programmable block\r\nTextor and CyberVic for their great script related contributions on Keen forums.\r\n\r\nWatch my Steam group: http://steamcommunity.com/groups/mmnews\r\nTwitter: https://twitter.com/MattsPlayCorner\r\nand Facebook: https://www.facebook.com/MattsPlayCorner1080p\r\nfor more crazy stuff from me in the future :)\r\n\r\nIf you think my work is worth it then feel free to donate. It is much appreciated! Thanks! :)\r\n[url=https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=YLQWKYT842E8C][img]https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif[/img][/url]",
                            "time_created": 1482356806,
                            "time_updated": 1486229768,
                            "visibility": 0,
                            "banned": 0,
                            "ban_reason": "",
                            "subscriptions": 51141,
                            "favorited": 2091,
                            "lifetime_subscriptions": 56419,
                            "lifetime_favorited": 2170,
                            "views": 77440,
                            "tags": [
                                {
                                    "tag": "ingameScript"
                                }
                            ]

                        }
                    ]

                }
            }*/
    }
}