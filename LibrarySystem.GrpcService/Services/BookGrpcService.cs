
using Grpc.Core;
using LibrarySystem.GrpcService;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using BookService = LibrarySystem.GrpcService.BookService;


public class BookGrpcService : BookService.BookServiceBase
{
    private readonly IBookServices _bookService;

    public BookGrpcService(IBookServices bookService)
    {
        _bookService = bookService;
    }
  
    public override async Task<GetAllBooksReply> GetAllBooks(GetAllBooksRequest request, ServerCallContext context)
    {
        var books = await _bookService.GetAllBooksAsync();

        var reply = new GetAllBooksReply();
        foreach (var book in books)
        {
            reply.Books.Add(new GetBookReply
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author               
            });
        }

        return reply;
    }

    public override async Task<GetMostBorrowedBooksReply> GetMostBorrowedBooks(GetMostBorrowedBooksRequest request, ServerCallContext context)
    {
        var books = await _bookService.GetMostBorrowedBooksAsync();

        var reply = new GetMostBorrowedBooksReply();
        foreach (var book in books)
        {
            reply.Books.Add(new BookWithBorrowCount
            {
                Title = book.Item1,
                Count = book.Item2
            });
        }

        return reply;
    }

    public override async Task<GetOtherBooksBorrowedBySamePeopleReply> GetOtherBooksBorrowedBySamePeople(GetOtherBooksBorrowedBySamePeopleRequest request, ServerCallContext context)
    {
        var booktitles = await _bookService.GetOtherBooksBorrowedBySamePeople(request.BookId);

        var reply = new GetOtherBooksBorrowedBySamePeopleReply();
        foreach (var booktitle in booktitles)
        {
            reply.Book.Add(new BookTitle
            {
                Booktitle = booktitle
            });
        }

        return reply;
    }

    public override async Task<GetReadingEstimateReply> GetReadingEstimate(GetReadingEstimateRequest request, ServerCallContext context)
    {
        var rate = await _bookService.GetReadingEstimate(request.BookId);
        var reply = new GetReadingEstimateReply();
        reply.AverageRate = rate;
        return reply;
    }

    public override async Task<GetBookStatsReply> GetBookStats(GetBookStatsRequest request, ServerCallContext context)
    {
        var books = await _bookService.GetBookStats(request.Id);

        var reply = new GetBookStatsReply
        {
            Borrowed =  books.Item2,
            CopiesTotal = books.Item1,
        };

        return reply;
    }





}

