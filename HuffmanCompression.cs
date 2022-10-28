using System.Text;

char[] s = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

public class Huffman
{
    public char[] alphabet;
    public List<HuffNode> Huff_struct;
    public string coded_text; 
    public string[] codes;

    public Huffman(char[] alphabet, string text)
    {
        int[] frequencies = GetCharFrequency(text, alphabet);
        List<HuffNode> Huff_struct = Compress(frequencies, alphabet);
        string coded_string = CodeText(Huff_struct, text);

        this.Huff_struct = Huff_struct;
        coded_text = coded_string;
        this.alphabet = alphabet;
        codes = (string[])Huff_struct.Select(x => x.code).ToArray();
    }


    int[] GetCharFrequency(string text_orig, char[] alphabet)
    {
        char curr_el;
        int alph_index = 0;
        int[] frequencies = new int[alphabet.Length];
        string text = text_orig;
        //Setting every frequency to 0
        Array.Clear(frequencies, 0, alphabet.Length);

        while (text != "")
        {
            curr_el = alphabet[alph_index];
            //Rising up frequency with each Removed elememt                  
            frequencies[alph_index] = text.Count(x => x == curr_el);
            text = text.Replace(curr_el.ToString(), "");            
            alph_index += 1;
        }
        return frequencies;
    }
    private (List<HuffNode>, int) MinFromList(List<(List<HuffNode>, int)> freq_list)
    {
        int min_val = (from item in freq_list select item.Item2).Min();
        (List<HuffNode>, int) to_remove = freq_list.Find(x => x.Item2 == min_val);
        return to_remove;
    }

    private void CompressIter(ref List<(List<HuffNode>, int)> freq_list) //LIst (node, freq) such that node is List of nodes
    {
        if (freq_list.Count > 1)
        {
            int index = 0;

            (List<HuffNode>, int) min1 = MinFromList(freq_list);            
            int min1_val = min1.Item2;            
            while (min1.Item1.Count > index)
            {
                min1.Item1[index].code = min1.Item1[index].code.Insert(0, "1");
                index += 1;
            }
            List<HuffNode> removed1 = min1.Item1;
            freq_list.Remove(min1);
            index = 0;

            (List<HuffNode>, int) min2 = MinFromList(freq_list);         
            int min2_val = min2.Item2;            
            while (min2.Item1.Count > index)
            {
                min2.Item1[index].code = min2.Item1[index].code.Insert(0, "0");
                index += 1;
            }
            List<HuffNode> removed2 = min2.Item1;           
            freq_list.Remove(min2);


            (List<HuffNode>, int) min3 = 
            (
                new List<HuffNode>(),
                min1_val + min2_val
            );

            min3.Item1.AddRange(removed1);
            min3.Item1.AddRange(removed2);

            freq_list.Add(min3);

            CompressIter(ref freq_list);
        }
    }

    private void MakeHuffNode(ref List<HuffNode> list, int[] freq, char item, int index)
    {   //Frequence indexes are sorted the same way as alphabet               
        var tup = new HuffNode(freq[index], item, "");
        list.Add(tup);                
    }

    private void MakeFreqList(ref List<(List<HuffNode>, int)> list, int[] freq, char[] alphabet)
    {   //Frequence indexes are sorted the same way as alphabet
        int index = 0;        
        foreach (char item in alphabet)
        {
            var node = new List<HuffNode>();
            MakeHuffNode(ref node,freq,item, index);

            var tup = (node, freq: freq[index]);
            list.Add(tup);
            index += 1;
        }
    }

    private List<HuffNode> Compress(int[] freq, char[] alphabet)
    {
        //var Huff_list = new List<HuffNode>();
        var freq_list = new List<(List<HuffNode>, int)>();

        //MakeHuffList(ref Huff_list, freq, alphabet);
        MakeFreqList(ref freq_list, freq, alphabet);

        CompressIter(ref freq_list);
        return freq_list[0].Item1;
    }    

    private string CodeText(List<HuffNode> Huff_struct, string text)
    {
        string result = text;
        foreach (var node in Huff_struct)
        {
           result = result.Replace(node.value.ToString(), node.code);
        }
        return result;
    }


    public void Print()
    {
        foreach(var Huff_node in Huff_struct)
        {
            Console.WriteLine(Huff_node);
        }
    }
    public string Encode()
    {
        StringBuilder result_string = new StringBuilder("");
        StringBuilder word = new StringBuilder("");
        foreach (char ch in coded_text)
        {
            word.Append(ch);
            string str_word = word.ToString();
            if (codes.Contains(str_word))
            {
                char letter = Huff_struct.Find(x => x.code == str_word).value;
                result_string.Append(letter);
                word.Clear();
            }
        }
        return result_string.ToString(); //Čti chacaktery ze stringu tak dlouho dokud nejaky string nebude kodem, pak replace, delej to dokola dokud neprojdes cely string
    }  
}

public class HuffNode
{
    public int freq;
    public char value;
    public string code;

    public HuffNode(int freq, char value, string code)
    {
        this.freq = freq;
        this.value = value;
        this.code = code;
    }

    public override string ToString()
    {
        return $"Character: {value} Code:{code}";
    }
}