using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class BookReview
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
    public float Rating { get; set; }
    public string Content { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<BookReview> b)
    {
        b.HasKey(s => s.Id);
        b.HasOne(s => s.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(s => s.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.User)
            .WithMany(s => s.Reviews)
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Content)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Rating)
            .IsRequired();
        b.HasCheckConstraint($"CK_{nameof(BookReview)}_{nameof(Rating)}", $"\"{nameof(Rating)}\" >= 0 AND \"{nameof(Rating)}\" <= 5");
        b.HasIndex(s => new {s.BookId, s.UserId})
            .IsUnique();
    }
    public static void CreateSeed(SeedingContext ctx)
    {
        BookReview review1= new(){
            Id=1,
            BookId=1,
            UserId=1,
            Rating=5,
            Content="This was a great read!!"
        };
        BookReview review2= new(){
            Id=2,
            BookId=1,
            UserId=2,
            Rating=5,
            Content="I just finished this book and like you I have read many WW2 novels. This one is a very different angle and I found it outstanding."
        };
         BookReview review3= new(){
            Id=3,
            BookId=1,
            UserId=3,
            Rating=3,
            Content="First let me say I really like this author she has a great writing style. This book I feel is not her best work.\nThe storyline centers on a group of people who are hiding from the Nazis in a forest with the help of a girl who spent her life growing up in the same forest after being kidnapped.\nIf you're looking for a WWII Resistance story this is not the book for you. If you're looking for a book that borders on a fantasy story with a little WWII worked into it then this is the book for you."
        };
         BookReview review4= new(){
            Id=4,
            BookId=1,
            UserId=4,
            Rating=1,
            Content="So much repetition...first time ever I have stopped reading a book in \"progress\" I think I decided to call it done at about page 120 on the kindle version. I really had trouble with the writing from the get go...and I read about 6 books a month, espeically World War II historical some fiction some non...but this was a OH NO for me. I say don't waste your time."
        };
          BookReview review5= new(){
            Id=5,
            BookId=1,
            UserId=5,
            Rating=4,
            Content="I think Kristen Harmel is a great writer and her subject matter is one I yearn to read. Somehow Yonas story seemed very chopped up to me compared to the plot outlines of her other books. I found the author’s afterward almost more interesting than the book. Overall, Harmel tells a very good story, this one just seemed to have too many starts of the same situation in the woods."
        };
         BookReview review6= new(){
            Id=6,
            BookId=1,
            UserId=6,
            Rating=5,
            Content="This book immediately drew me in to the story of a forest girl and her survival. I fell in love with her and couldn’t put the book down to see where her struggles took her. Beautifully written, heroism, grief and love all wrapped up together, wonderful read."
        };
         BookReview review7= new(){
            Id=7,
            BookId=1,
            UserId=7,
            Rating=3,
            Content="It's an odd thing to say about the subject matter, but this book never really grabbed my attention strongly from start to finish. Obviously the story itself is fascinating, especially given the factual historical context, but the way it was written wasn't terribly compelling. I'm a little baffled by the endless glowing reviews. It was one of those books where when I finished it I was happy it was over and eagerly reaching for the next one in my pile."
        };
         BookReview review8= new(){
            Id=8,
            BookId=1,
            UserId=8,
            Rating=5,
            Content="Couldn't put it down I read it in two days, this is a piece of history I honestly didn't know anything about, I have read all of her books and they just keep getting better - I can hardly wait for the next one. I tell everybody I know to read Kristen Harmel Books"
        };
         BookReview review9= new(){
            Id=9,
            BookId=1,
            UserId=9,
            Rating=5,
            Content="Beautiful book, thoroughly enjoyed every moment of it!"
        };
         BookReview review10= new(){
            Id=10,
            BookId=1,
            UserId=10,
            Rating=5,
            Content="This is a captivating story, and so well researched and written. The forest came alive for me, and the drama so real, so heartrending, so tragic and so ultimately triumphant. I have been in the big and little ghettoes, in Warsaw…I have been to Mir Castle which served as a ghetto in Belarus…I have stood and wept over mass graves in both Poland and Belarus….so yes, the story came to lord for me. Thank you for sharing with us, that we might never forget."
        };
        List<BookReview>reviews=new(){review1,review2,review3,review4,review5,review6,review7,review8,review9,review10};
        ctx.BooksReviews.AddRange(reviews);
        //add ratings to books
        foreach(var r in ctx.BooksReviews)
        {
            var _book=ctx.Books.FirstOrDefault(b=>b.Id==r.BookId);
            _book.RatersCount++;
            _book.RatingSum+=r.Rating;
        }
       // var _book =ctx.Books.FirstOrDefault(b=>b.Id==review1.BookId);
        //_book.RatersCount++;
        //_book.RatingSum+=review1.Rating;
        /*Random rand = new();
        var users = ctx.Users.ToArray();
        foreach (var book in ctx.Books)
        {
            int reviewCount = rand.Next(users.Length);
            book.RatersCount += reviewCount;
            for (int i = 0; i < reviewCount; i++)
            {
                BookReview review = new()
                {
                    BookId = book.Id,
                    Content = rand.NextText(),
                    Rating = rand.Next(6),
                    UserId = rand.NextElementAndSwap(users, users.Length - (i + 1)).Id,
                    Id = ctx.BooksReviews.Count + 1
                };
                book.RatingSum += review.Rating;
                ctx.BooksReviews.Add(review);
            }
        }*/
    }
}