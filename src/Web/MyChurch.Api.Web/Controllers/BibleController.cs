using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Bible.Queries.GetAllBibleVersions;
using MyChurch.Application.Bible.Queries.GetBooksByVersion;
using MyChurch.Application.Bible.Queries.GetChaptersByBook;
using MyChurch.Application.Bible.Queries.GetChaptersByBookName;
using MyChurch.Application.Bible.Queries.GetVerseByChapterAndNumber;
using MyChurch.Application.Bible.Queries.GetVersesByChapter;
using MyChurch.Application.Bible.Queries.GetVersesByReference;

namespace MyChurch.Api.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BibleController : BaseController
    {
        /// <summary>
        /// Lista todas as versões da Bíblia disponíveis.
        /// </summary>
        [HttpGet("versions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVersions()
        {
            var query = AuthorizationRequestCreate<GetAllBibleVersionsQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os livros de uma versão da Bíblia.
        /// </summary>
        [HttpGet("versions/{versionId}/books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBooks(int versionId)
        {
            var query = AuthorizationRequestCreate<GetBooksByVersionQuery>();
            query.VersionId = versionId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os capítulos de um livro.
        /// </summary>
        [HttpGet("books/{bookId}/chapters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChapters(int bookId)
        {
            var query = AuthorizationRequestCreate<GetChaptersByBookQuery>();
            query.BookId = bookId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os capítulos de um livro pelo nome do livro e ID da versão.
        /// </summary>
        [HttpGet("versions/{versionId}/books/{bookName}/chapters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChaptersByBookName(int versionId, string bookName)
        {
            var query = AuthorizationRequestCreate<GetChaptersByBookNameQuery>();
            query.VersionId = versionId;
            query.BookName = bookName;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retorna um versículo específico de um capítulo.
        /// </summary>
        [HttpGet("chapters/{chapterId}/verses/{verseNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVerse(int chapterId, int verseNumber)
        {
            var query = AuthorizationRequestCreate<GetVerseByChapterAndNumberQuery>();
            query.ChapterId = chapterId;
            query.VerseNumber = verseNumber;
            var result = await Mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os versículos de um capítulo.
        /// </summary>
        [HttpGet("chapters/{chapterId}/verses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVerses(int chapterId)
        {
            var query = AuthorizationRequestCreate<GetVersesByChapterQuery>();
            query.ChapterId = chapterId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os versículos por referência bíblica (versão, livro e capítulo).
        /// </summary>
        [HttpGet("versions/{versionId}/books/{bookName}/chapters/{chapterNumber}/verses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVersesByReference(int versionId, string bookName, int chapterNumber)
        {
            var query = AuthorizationRequestCreate<GetVersesByReferenceQuery>();
            query.VersionId = versionId;
            query.BookName = bookName;
            query.ChapterNumber = chapterNumber;
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}