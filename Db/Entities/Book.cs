using Mashawi.Resources;
using Mashawi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkiaSharp;
namespace Mashawi.Db.Entities;
public enum BookLanguage { Arabic, English }
public enum BookGenre { Fantasy, Romance, Historical, Horror, Classics, Fiction }
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public decimal Price { get; set; }
    public float RatersCount { get; set; }
    public float RatingSum { get; set; }
    public float Rating => RatersCount == 0 ? 0 : RatingSum / RatersCount;
    public bool IsUsed { get; set; }
    public string Description { get; set; }
    public BookLanguage Language { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime AddedDate { get; set; }
    public BookGenre Genre { get; set; }
    public int Stock { get; set; }
    public int Sold { get; set; }
    public ICollection<BookReview> Reviews { get; set; }


    public static void ConfigureEntity(EntityTypeBuilder<Book> b)
    {
        b.HasKey(s => s.Id);
        b.Ignore(s => s.Rating);
        b.Property(s => s.Title)
            .IsUnicode()
            .IsRequired();
        b.Property(s => s.Sold)
            .IsRequired();
        b.Property(s => s.Isbn)
            .IsRequired();
        b.HasOne(s => s.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(s => s.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Price)
            .IsRequired();
        b.Property(s => s.RatersCount)
            .IsRequired()
            .HasDefaultValue(0);
        b.Property(s => s.RatingSum)
            .IsRequired()
            .HasDefaultValue(0);
        b.Property(s => s.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);
        b.Property(s => s.Description)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Language)
            .IsRequired()
            .HasConversion<byte>();
        b.Property(s => s.PublishDate)
            .IsRequired();
            b.Property(s => s.AddedDate)
            .IsRequired();
        b.Property(s => s.Genre)
            .IsRequired()
            .HasConversion<byte>();
        b.Property(s => s.Stock)
            .IsRequired();

        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(RatersCount)}", $"\"{nameof(RatersCount)}\" >= 0");
        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(RatingSum)}", $"\"{nameof(RatingSum)}\" >= 0");
        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(Stock)}", $"\"{nameof(Stock)}\" >= 0");
        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(Sold)}", $"\"{nameof(Sold)}\" >= 0");
    }

    public static async Task CreateSeedFiles(IServiceProvider sp, SeedingContext seedingContext)
    {
      /*  using SKPaint paint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill,
            HintingLevel = SKPaintHinting.Full,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            TextSize = 10,
            StrokeWidth = 10
        };
        Random rand = new();*/
        var fileManager = sp.GetRequiredService<BookFileManager>();

        async Task GenerateBookPicture(Book b)
        {
           /* using SKBitmap bmp = new(200, 200);
            using (SKCanvas can = new(bmp))
            {
                can.Clear(new SKColor((uint)rand.Next(100, int.MaxValue)));
                can.Flush();
            }
            using var jpgData = bmp.Encode(SKEncodedImageFormat.Jpeg, 100);
            await using var jpgStream = jpgData.AsStream();*/
            using var jpgStream = AppResourcesManager.GetStream($"BookCovers.{b.Id}.jpg");
            await fileManager.SaveFile(b.Id, jpgStream).ConfigureAwait(false);
        }

        foreach (var book in seedingContext.Books)
        {
            await GenerateBookPicture(book).ConfigureAwait(false);
        }
    }
    public static void CreateSeed(SeedingContext ctx)
    {
        Book book11 = new(){
            Id=1,
            AuthorId=1,
            Title="The Forest of Vanishing Stars",
            Genre=BookGenre.Historical,
            Language=BookLanguage.English,
            Isbn="9781638080442",
            IsUsed=false,
            Price=18.99m,
            Stock=20,
            PublishDate=new DateTime(2021,9,21).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-1),
            //AddedDate=DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),          
            Sold=0,
            Description="After being stolen from her wealthy German parents and raised in the unforgiving wilderness of eastern Europe, a young woman finds herself alone in 1941 after her kidnapper dies. Her solitary existence is interrupted, however, when she happens upon a group of Jews fleeing the Nazi terror. Stunned to learn what's happening in the outside world, she vows to teach the group all she can about surviving in the forest--and in turn, they teach her some surprising lessons about opening her heart after years of isolation. But when she is betrayed and escapes into a German-occupied village, her past and present come together in a shocking collision that could change everything."
                        +"Inspired by incredible true stories of survival against staggering odds, and suffused with the journey-from-the-wilderness elements that made Where the Crawdads Sing a worldwide phenomenon, The Forest of Vanishing Stars is a heart-wrenching and suspenseful novel from the #1 internationally bestselling author whose writing has been hailed as \"sweeping and magnificent\" (Fiona Davis, New York Times bestselling author), \"immersive and evocative\" (Publishers Weekly), and \"gripping\" (Tampa Bay Times).",
        };
       // ctx.Books.Add(book11);
         Book book21 = new(){
            Id=2,
            AuthorId=1,
            Title="The Life Intended",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9781410477897",
            IsUsed=false,
            Price=16.99m,
            Stock=10,
            PublishDate=new DateTime(2014,12,30).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-2),
            Sold=0,
            Description="After her husband's sudden death over ten years ago, Kate Waithman never expected to be lucky enough to find another love of her life. But now she's planning her second walk down the aisle to a perfectly nice man. So why isn't she more excited? At first, Kate blames her lack of sleep on stress. But when she starts seeing Patrick, her late husband, in her dreams, she begins to wonder if she's really ready to move on. Is Patrick trying to tell her something? Attempting to navigate between dreams and reality, Kate must uncover her husband's hidden message. Her quest leads her to a sign language class and into the New York City foster system, where she finds rewards greater than she could have imagined." 
       };
       //ctx.Books.Add(book21);

      Book book12 = new(){
            Id=3,
            AuthorId=2,
            Title="Accidentally Engaged",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9781538734988",
            IsUsed=false,
            Price=15.99m,
            Stock=10,
            PublishDate=new DateTime(2021,3,2).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-3),
            Sold=0,
            Description="When it comes to bread, Reena Manji knows exactly what she's doing. She treats her sourdough starters like (somewhat unruly) children. But when it comes to Reena's actual family--and their constant meddling in her life--well, that recipe always ends in disaster. Now Reena's parents have found her yet another potential Good Muslim Husband. This one has the body of Captain America, a delicious British accent, and lives right across the hall. He's the perfect, mouthwatering temptation . . . and completely ruined by the unwelcome side dish of parental interference. Reena refuses to marry anyone who works for her father. She won't be attracted to Nadim's sweet charm or gorgeous lopsided smile. That is, until the baking opportunity of a lifetime presents itself: a couples' cooking competition with the prize of her dreams. Reena will do anything to win--even asking Nadim to pretend they're engaged. But when it comes to love, baking your bread doesn't always mean you get to eat it too."
        };

    Book book13=new(){
            Id=4,
            AuthorId=3,
            Title="First Love, Take Two",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9781538733363",
            IsUsed=false,
            Price=15.99m,
            Stock=10,
            PublishDate=new DateTime(2021,9,9).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-4),
            Sold=0,
            Description="On the verge of realizing her dream of being a doctor, Preeti Patel should be ecstatic. But between the stress of her residency, trying to find a job, and managing her traditional, no-boundaries family, Preeti's anxiety is through the roof. Relationships and love aren't even an option. Fortunately, Preeti's finally found a new place to stay . . . only to discover that her new roommate is her ex. Preeti never quite got over Daniel Thompson. Super-hot, plenty of swagger, amazing cook--the guy is practically perfect. And if it weren't for their families, there might have been a happily ever after. But it's hard to keep her sanity and libido in check when the man of her dreams is sleeping mere feet away. Can Preeti and Daniel find a way to stand up and fight for each other one last time . . . before they lose their second chance?"
        };
          Book book14=new(){
            Id=5,
            AuthorId=4,
            Title="Just Haven't Met You Yet",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9780593331521",
            IsUsed=false,
            Price=14.75m,
            Stock=10,
            PublishDate=new DateTime(2021,11,9).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-5),
            Sold=0,
            Description="Hopeless romantic and lifestyle reporter Laura's business trip to the Channel Islands isn't off to a great start. After an embarrassing encounter with the most attractive man she's ever seen in real life, she arrives at her hotel and realizes she's grabbed the wrong suitcase from the airport. Her only consolation is its irresistible contents, each of which intrigues her more and more. The owner of this suitcase is clearly Laura's dream man. Now, all she has to do is find him. Besides, what are the odds that she'd find The One on the same island where her parents first met and fell in love, especially as she sets out to write an article about their romance? Commissioning surly cab driver Ted to ferry her around seems like her best bet in both tracking down the mystery suitcase owner and retracing her parents' footsteps. But as Laura's mystery man proves difficult to find--and as she uncovers family secrets--she may have to reimagine the life, and love, she always thought she wanted."
        }; 
         Book book15=new(){
            Id=6,
            AuthorId=5,
            Title="Red, White & Royal Blue",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9788427218697",
            IsUsed=false,
            Price=15.65m,
            Stock=10,
            PublishDate=new DateTime(2019,5,14).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-6),
            Sold=0,
            Description =" What happens when America's First Son falls in love with the Prince of Wales? When his mother became President, Alex Claremont-Diaz was promptly cast as the American equivalent of a young royal. Handsome, charismatic, genius--his image is pure millennial-marketing gold for the White House. There's only one problem: Alex has a beef with the actual prince, Henry, across the pond. And when the tabloids get hold of a photo involving an Alex-Henry altercation, U.S./British relations take a turn for the worse. Heads of family, state, and other handlers devise a plan for damage control: staging a truce between the two rivals. What at first begins as a fake, Instragramable friendship grows deeper, and more dangerous, than either Alex or Henry could have imagined. Soon Alex finds himself hurtling into a secret romance with a surprisingly unstuffy Henry that could derail the campaign and upend two nations and begs the question: Can love save the world after all? Where do we find the courage, and the power, to be the people we are meant to be? And how can we learn to let our true colors shine through? Casey McQuiston's Red, White & Royal Blue proves: true love isn't always diplomatic." 
       }; 

       Book book16=new(){
            Id=7,
            AuthorId=6,
            Title="Get a Life, Chloe Brown",
            Genre=BookGenre.Romance,
            Language=BookLanguage.English,
            Isbn="9780063215375",
            IsUsed=false,
            Price=14.70m,
            Stock=10,
            PublishDate=new DateTime(2019,11,5).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-7),
            Sold=0,
            Description="Chloe Brown is a chronically ill computer geek with a goal, a plan, and a list. After almost--but not quite--dying, she's come up with seven directives to help her \"Get a Life\", and she's already completed the first: finally moving out of her glamorous family's mansion. The next items? \n1)Enjoy a drunken night out.\n2)Ride a motorcycle.\n3)Go camping.\n4)Have meaningless but thoroughly enjoyable sex.\n5)Travel the world with nothing but hand luggage.\n6)And... do something bad. But it's not easy being bad, even when you've written step-by-step guidelines on how to do it correctly. What Chloe needs is a teacher, and she knows just the man for the job. Redford 'Red' Morgan is a handyman with tattoos, a motorcycle, and more sex appeal than ten-thousand Hollywood heartthrobs. He's also an artist who paints at night and hides his work in the light of day, which Chloe knows because she spies on him occasionally. Just the teeniest, tiniest bit.\nBut when she enlists Red in her mission to rebel, she learns things about him that no spy session could teach her. Like why he clearly resents Chloe's wealthy background. And why he never shows his art to anyone. And what really lies beneath his rough exterior..."
       };
       Book book17=new(){
            Id=8,
            AuthorId=7,
            Title="The Eyes of the Dragon",
            Genre=BookGenre.Fantasy,
            Language=BookLanguage.English,
            Isbn="9781501192203",
            IsUsed=false,
            Price=18.0m,
            Stock=10,
            PublishDate=new DateTime(2019,1,9).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-8),
            Sold=0,
            Description="\"Once, in a kingdom called Delain, there was a king with two sons....\" \nThus begins one of the most unique tales that master storyteller Stephen King has ever written--a sprawling fantasy of dark magic and the struggle for absolute power that utterly transforms the destinies of two brothers born into royalty. Through this enthralling masterpiece of mythical adventure, intrigue, and terror, you will thrill to this unforgettable narrative filled with relentless, wicked enchantment, and the most terrible of secrets...."
       };
       Book book18=new(){
            Id=9,
            AuthorId=8,
            Title="The Hobbit: Or There and Back Again",
            Genre=BookGenre.Fantasy,
            Language=BookLanguage.English,
            Isbn="9780547928227",
            IsUsed=false,
            Price=15.65m,
            Stock=10,
            PublishDate=new DateTime(2012,9,18).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-9),
            Sold=0,
            Description="A great modern classic and the prelude to The Lord of the Rings. Bilbo Baggins is a hobbit who enjoys a comfortable, unambitious life, rarely traveling any farther than his pantry or cellar. But his contentment is disturbed when the wizard Gandalf and a company of dwarves arrive on his doorstep one day to whisk him away on an adventure. They have launched a plot to raid the treasure hoard guarded by Smaug the Magnificent, a large and very dangerous dragon. Bilbo reluctantly joins their quest, unaware that on his journey to the Lonely Mountain he will encounter both a magic ring and a frightening creature known as Gollum.\n\"A glorious account of a magnificent adventure, filled with suspense and seasoned with a quiet humor that is irresistible . . . All those, young or old, who love a fine adventurous tale, beautifully told, will take The Hobbit to their hearts.\" - New York Times Book Review"
       };
       Book book19=new(){
            Id=10,
            AuthorId=9,
            Title="The Catcher in the Rye",
            Genre=BookGenre.Classics,
            Language=BookLanguage.English,
            Isbn="9780241900970",
            IsUsed=false,
            Price=21.99m,
            Stock=10,
            PublishDate=new DateTime(2001,1,30).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-10),
            Sold=0,
            Description="The hero-narrator of The Catcher in the Rye is an ancient child of sixteen, a native New Yorker named Holden Caulfield. Through circumstances that tend to preclude adult, secondhand description, he leaves his prep school in Pennsylvania and goes underground in New York City for three days. The boy himself is at once too simple and too complex for us to make any final comment about him or his story. Perhaps the safest thing we can say about Holden is that he was born in the world not just strongly attracted to beauty but, almost, hopelessly impaled on it. There are many voices in this novel: children's voices, adult voices, underground voices-but Holden's voice is the most eloquent of all. Transcending his own vernacular, yet remaining marvelously faithful to it, he issues a perfectly articulated cry of mixed pain and pleasure. However, like most lovers and clowns and poets of the higher orders, he keeps most of the pain to, and for, himself. The pleasure he gives away, or sets aside, with all his heart. It is there for the reader who can handle it to keep."
       };
        Book book110=new(){
            Id=11,
            AuthorId=10,
            Title="Lord of the Flies",
            Genre=BookGenre.Classics,
            Language=BookLanguage.English,
            Isbn="9780399529207",
            IsUsed=false,
            Price=20.40m,
            Stock=10,
            PublishDate=new DateTime(1999,10,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-10),
            Sold=0,
            Description="At the dawn of the next world war, a plane crashes on an uncharted island, stranding a group of schoolboys. At first, with no adult supervision, their freedom is something to celebrate; this far from civilization the boys can do anything they want. Anything. They attempt to forge their own society, failing, however, in the face of terror, sin and evil. And as order collapses, as strange howls echo in the night, as terror begins its reign, the hope of adventure seems as far from reality as the hope of being rescued. Labeled a parable, an allegory, a myth, a morality tale, a parody, a political treatise, even a vision of the apocalypse, Lord of the Flies is perhaps our most memorable novel about \"the end of innocence, the darkness of man???s heart.\""
        };
          Book book410=new(){    //Exitst in both new and used
            Id=14,
            AuthorId=10,
            Title="Free Fall",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9780156028233",
            IsUsed=false,
            Price=16.14m,
            Stock=10,
            PublishDate=new DateTime(2003,6,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-12),
            Sold=0,
            Description="\"I was standing up, pressed back against the wall, trying not to breathe. I got there in the one movement my body made. My body had many hairs on legs and belly and chest and head, and each had its own life; each inherited a hundred thousand years of loathing and fear for things that scuttle or slide or crawl.\" from Free Fall"
         };
        //USED BOOKS STARTS FROM HERE
        Book book210=new(){
            Id=12,
            AuthorId=10,
            Title="The Inheritors",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9788445074411",
            IsUsed=true,
            Price=15.95m,
            Stock=10,
            PublishDate=new DateTime(1963,8,28).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-11),
            Sold=0,
            Description="When the spring came the people - what was left of them - moved back by the old paths from the sea. But this year strange things were happening, terrifying things that had never happened before. Inexplicable sounds and smells; new, unimaginable creatures half glimpsed through the leaves. What the people didn't, and perhaps never would, know, was that the day of their people was already over."    
        };
         Book book310=new(){    //Exitst in both new and used
            Id=13,
            AuthorId=10,
            Title="Free Fall",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9780156028233",
            IsUsed=true,
            Price=4.49m,
            Stock=10,
            PublishDate=new DateTime(2003,6,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-12),
            Sold=0,
            Description="\"I was standing up, pressed back against the wall, trying not to breathe. I got there in the one movement my body made. My body had many hairs on legs and belly and chest and head, and each had its own life; each inherited a hundred thousand years of loathing and fear for things that scuttle or slide or crawl.\" from Free Fall"
         };

         Book book111=new(){
            Id=15,
            AuthorId=11,
            Title="The Unraveling of Luna Forester",
            Genre=BookGenre.Horror,
            Language=BookLanguage.English,
            Isbn="9781916893221",
            IsUsed=true,
            Price=7.59m,
            Stock=10,
            PublishDate=new DateTime(2021,12,9).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-13),
            Sold=0,
            Description="MATTHEW HAS ALWAYS PROTECTED LUNA???S SECRET.\nNOW HE MUST PROTECT HER LIFE.\nWhen his best friend Luna is found catatonic after a devastating house fire, Matthew begins to fall apart.\n\"Take care of them, Matthew.\"\n\"Always.\"\n\"Protect my secret.\"\n\"Of course.\"\nThat???s the promise he made to her only three nights ago. A solemn vow to protect their large found family of humans and supernatural creatures alike.\nFated to love her for the rest of his life, and unwilling to break his solemn vow, Matthew knows the only person who can help Luna is her grandmother. Through the woods they must go, just like a fairytale. But the forest is filled with deadly peril: poisonous black moss, chimeras, and worst of all, members of their family who don't want them to continue.\nAs they are picked off one by one, Matthew races to get Luna to safety, all the time doubting everything he thought was true.\nCan Matthew untangle the twisted threads of Luna???s secret before he himself unravels?"
         };
         Book book112=new(){
            Id=16,
            AuthorId=12,
            Title="A Wish Too Dark And Kind",
            Genre=BookGenre.Horror,
            Language=BookLanguage.English,
            Isbn="9781737019213",
            IsUsed=true,
            Price=9.73m,
            Stock=10,
            PublishDate=new DateTime(2021,11,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-14),
            Sold=0,
            Description="Among the immortals that inhabit our world, Arnaud Demeure is known as the man who can fulfill your one true wish or who can also conjure your worst nightmares.\nEight invitations are sent to eight immortals, and when Arnaud Demeure hosts a party, no one refuses his request. Why have they been summoned? Is it for a celebration or does a sinister fate await them? After all, in the ways of Magick, a party can also be a ritual to end the world.\nAs the mystery deepens, the attendees must overcome their personal grudges to unravel the threads of Demeure's grand plan that has been centuries in the making. But, with one of the guests secretly working with their host to sabotage the group's every move, it seems impossible to look behind the curtain to learn Demeure's true intentions.\nWith each guest hiding dark secrets and darker intentions, will they be able to uncover Demeure's mysterious motives or will the party prove to be the deadly nightmare that they each fear?"
         };
           Book book510=new(){
            Id=17,
            AuthorId=10,
            Title="Darkness Visible",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9780156239318",
            IsUsed=true,
            Price=3.70m,
            Stock=10,
            PublishDate=new DateTime(1999,2,15).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-15),
            Sold=0,
            Description="A dazzlingly dark novel by the Nobel Laureate.\nAt the height of the London blitz, a naked child steps out of an all-consuming fire. Miraculously saved yet hideously scarred, tormented at school and at work, Matty becomes a wanderer, a seeker after some unknown redemption. Two more lost children await him: twins as exquisite as they are loveless. Toni dabbles in political violence, Sophy in sexual tyranny. As Golding weaves their destinies together, as he draws them toward a final conflagration, his book lights up both the inner and outer darknesses of our time."
           };
            Book book610=new(){
            Id=18,
            AuthorId=10,
            Title="The Double Tongue",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9780374143299",
            IsUsed=true,
            Price=3.79m,
            Stock=10,
            PublishDate=new DateTime(1999,10,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-15),
            Sold=0,
            Description="The Double Tongue is William Golding's last and perhaps most superbly imaginative novel. It is a fictional memoir of an aged prophetess at Delphi, the most sacred oracle of ancient Greece, just prior to Greece's domination by the Roman Empire. As a young girl, Arieka is ugly, unconventional, a source of great shame to her uppity parents, who fear they'll never marry her off. But she is saved by Ionides, the High Priest of the Delphic temple, who detects something of a seer (and a friend) in her and whisks her off to the shrine to become the Pythia - the earthly voice of the god Apollo. Arieka has now spent a lifetime at the mercy of a god, a priest, and her devotees, and has witnessed firsthand the decay of Delphi's fortunes and its influence in the world. Her reflections on the mysteries of the oracle, which her own weird gifts embody, are matched by her feminine insight into the human frailties of the High Priest himself, a true Athenian with a wicked sense of humor, whose intriguing against the Romans brings about humiliation and disaster. This extraordinary short novel, left in draft at the author's death in 1993, is a psychological and historical triumph. Golding has created a vivid and comic picture of ancient Greek society as well as an absolutely convincing portrait of a woman's experience, something rare in the Golding oeuvre. Arieka the Pythia is one of his finest creations.\nLeft in draft at the author's death in 1993, this extraordinary short novel is a psychological and historical triumph. An aged prophetess at Delphi, the most sacred oracle in ancient Greece, looks back over her strange life as the Pythia, the voice of the god Apollo. Golding was the author of Lord of the Flies, and a Nobel Laureate."
            };
            Book book710=new(){
            Id=19,
            AuthorId=10,
            Title="The Paper Men",
            Genre=BookGenre.Fiction,
            Language=BookLanguage.English,
            Isbn="9780156708005",
            IsUsed=true,
            Price=3.74m,
            Stock=10,
            PublishDate=new DateTime(1999,12,1).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-16),
            Sold=0,
            Description="English novelist Wilfred Barclay, who has known fame, success, and fortune, is in crisis. He faces a drinking problem slipping over the borderline into alcoholism, a dead marriage, and the incurable itch of middle age lust. But the final, unbearable irritation is American Professor of English Literature Rick L. Tucker, who is implacable in his determinition to become The Barclay Man: authorized biographer, editor of the posthumous papers and the recognized authority."
            };
            Book book113=new(){
            Id=20,
            AuthorId=13,
            Title="Lightning Down: A World War II Story of Survival",
            Genre=BookGenre.Historical,
            Language=BookLanguage.English,
            Isbn="9781250151261",
            IsUsed=true,
            Price=14.99m,
            Stock=10,
            PublishDate=new DateTime(2021,11,2).ToUniversalTime(),
            AddedDate=DateTime.UtcNow.Date.AddDays(-17),
            Sold=0,
            Description="On August 13, 1944, Joe Moser set off on his forty-fourth combat mission over occupied France. Soon, he would join almost 170 other Allied airmen as prisoners in Buchenwald, one of the most notorious and deadly of Nazi concentration camps. Tom Clavin's Lightning Down tells this largely untold and riveting true story.\nMoser was just twenty-two years old, a farm boy from Washington State who fell in love with flying. During the War he realized his dream of piloting a P-38 Lightning, one of the most effective weapons the Army Air Corps had against the powerful German Luftwaffe. But on that hot August morning he had to bail out of his damaged, burning plane. Captured immediately, Moser???s journey into hell began.\nMoser and his courageous comrades from England, Canada, New Zealand, and elsewhere endured the most horrific conditions during their imprisonment... until the day the orders were issued by Hitler himself to execute them. Only a most desperate plan would save them.\nThe page-turning momentum of Lightning Down is like that of a thriller, but the stories of imprisoned and brutalized airmen are true and told in unforgettable detail, led by the distinctly American voice of Joe Moser, who prays every day to be reunited with his family.\nLightning Down is a can???t-put-it-down inspiring saga of brave men confronting great evil and great odds against survival."
            };
        List<Book> books= new(){book11,book21,book12,book13,book14,book15,book16,book17,book18,book19,book110,book210,book310,book410,book111,book112,book510,book610,book710,book113};
        ctx.Books.AddRange(books);

        
       /* Random rand = new();
        var languages = Enum.GetValues<BookLanguage>();
        var genres = Enum.GetValues<BookGenre>();
        for (int i = 1; i <= 200; i++)
        {
            Book book = new()
            {
                Id = ctx.Books.Count + 1,
                AuthorId = rand.NextElement(ctx.Authors).Id,
                Description = rand.NextText(),
                Genre = rand.NextElement(genres),
                Isbn = $"{rand.NextNumber(3)}-{rand.NextNumber(5)}-",
                Language = rand.NextElement(languages),
                PublishDate = DateTime.UtcNow - TimeSpan.FromDays(rand.Next(2, 3000)),
                Title = rand.NextText(),
                IsUsed = rand.NextBool(),
                Price = rand.Next(2, 100),
                Stock = rand.Next(300),
                Sold = 0
            };
            book.Isbn += book.Id.ToString();
            book.Isbn = book.Isbn.PadRight(13, '0');
            ctx.Books.Add(book);
        }*/
    }

}