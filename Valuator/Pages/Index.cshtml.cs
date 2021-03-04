using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        private double Similarity(string text)
        {
            var keys = _storage.GetKeys();
            foreach(string idText in keys)
            {
                if(idText.Substring(0,5) == "TEXT-")
                {
                    string simpleText = _storage.Load(idText);
                    if (text == simpleText)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        private double Rank(string text)
        {
            int noLettersCounter = 0;
            foreach(char ch in text)
            {
                if(!Char.IsLetter(ch))
                {
                    noLettersCounter++;
                }
            }
            return (double)noLettersCounter / text.Length;
        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            string similarity = Similarity(text).ToString();
            _storage.Store(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            string rankKey = "RANK-" + id;
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            string rank = Rank(text).ToString();
            _storage.Store(rankKey, rank);

            return Redirect($"summary?id={id}");
        }
    }
}
