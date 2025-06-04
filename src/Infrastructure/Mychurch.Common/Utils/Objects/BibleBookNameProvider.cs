namespace Mychurch.Common.Utils.Objects
{
    public static class BibleBookNameProvider
    {
        // Dicionário: abreviação -> nome completo (Português)
        private static readonly Dictionary<string, string> Portuguese = new()
        {
            // Antigo Testamento
            ["gn"] = "Gênesis",
            ["ex"] = "Êxodo",
            ["lv"] = "Levítico",
            ["nm"] = "Números",
            ["dt"] = "Deuteronômio",
            ["js"] = "Josué",
            ["jz"] = "Juízes",
            ["rt"] = "Rute",
            ["1sm"] = "1 Samuel",
            ["2sm"] = "2 Samuel",
            ["1rs"] = "1 Reis",
            ["2rs"] = "2 Reis",
            ["1cr"] = "1 Crônicas",
            ["2cr"] = "2 Crônicas",
            ["ed"] = "Esdras",
            ["ne"] = "Neemias",
            ["et"] = "Ester",
            ["jó"] = "Jó",
            ["sl"] = "Salmos",
            ["pv"] = "Provérbios",
            ["ec"] = "Eclesiastes",
            ["ct"] = "Cânticos",
            ["is"] = "Isaías",
            ["jr"] = "Jeremias",
            ["lm"] = "Lamentações",
            ["ez"] = "Ezequiel",
            ["dn"] = "Daniel",
            ["os"] = "Oseias",
            ["jl"] = "Joel",
            ["am"] = "Amós",
            ["ob"] = "Obadias",
            ["jn"] = "Jonas",
            ["mq"] = "Miquéias",
            ["na"] = "Naum",
            ["hc"] = "Habacuque",
            ["sf"] = "Sofonias",
            ["ag"] = "Ageu",
            ["zc"] = "Zacarias",
            ["ml"] = "Malaquias",

            // Novo Testamento
            ["mt"] = "Mateus",
            ["mc"] = "Marcos",
            ["lc"] = "Lucas",
            ["jo"] = "João",
            ["atos"] = "Atos dos Apóstolos",
            ["rm"] = "Romanos",
            ["1co"] = "1 Coríntios",
            ["2co"] = "2 Coríntios",
            ["gl"] = "Gálatas",
            ["ef"] = "Efésios",
            ["fp"] = "Filipenses",
            ["cl"] = "Colossenses",
            ["1ts"] = "1 Tessalonicenses",
            ["2ts"] = "2 Tessalonicenses",
            ["1tm"] = "1 Timóteo",
            ["2tm"] = "2 Timóteo",
            ["tt"] = "Tito",
            ["fm"] = "Filemom",
            ["hb"] = "Hebreus",
            ["tg"] = "Tiago",
            ["1pe"] = "1 Pedro",
            ["2pe"] = "2 Pedro",
            ["1jo"] = "1 João",
            ["2jo"] = "2 João",
            ["3jo"] = "3 João",
            ["jd"] = "Judas",
            ["ap"] = "Apocalipse"
        };

        public static string GetBookName(string abbrev, string language = "pt")
        {
            var abbr = abbrev.ToLowerInvariant();
            return language switch
            {
                "pt" => Portuguese.TryGetValue(abbr, out var name) ? name : abbr.ToUpper(),
                // Adicione outros idiomas aqui, se necessário
                _ => abbr.ToUpper()
            };
        }
    }
}
