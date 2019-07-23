using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System;
using MineServer.Objects;

namespace MineServer.MCFormat
{
    public class FormatParser
    {
        private readonly TokenStream tokenStream;

        public FormatParser(TokenStream tokenStream)
        {
            this.tokenStream = tokenStream;
        }

        private ChatObject makeChatObject(char option, ChatObject parent)
        {
            var co = new ChatObject
            {
                Bold = parent.Bold,
                Color = parent.Color,
                Italic = parent.Italic,
                Obfuscated = parent.Obfuscated,
                Strikethrough = parent.Strikethrough,
                Underlined = parent.Underlined
            };

            switch (option)
            {
                case char c when MineColor.ColorChars.Contains(c):
                    co.Color = MineColor.Find(c);
                    break;
                case 'k':
                    co.Obfuscated = true;
                    break;
                case 'l':
                    co.Bold = true;
                    break;
                case 'm':
                    co.Strikethrough = true;
                    break;
                case 'n':
                    co.Underlined = true;
                    break;
                case 'o':
                    co.Italic = true;
                    break;
            }

            return co;
        }

        public ChatObject Parse()
        {
            return ParseChat(null, null);
        }

        private readonly Stack<ChatObject> rootAddStack = new Stack<ChatObject>();
        private ChatObject ParseChat(ChatObject parent, ChatObject root)
        {
            switch (tokenStream.NextToken())
            {
                case Token t when t.Type == TokenType.Format && t.Value[0] == 'r':
                    var _cobj = new ChatObject();
                    _cobj.Children = new ChatObject[] { ParseChat(_cobj, root ?? (root = _cobj)) };
                    rootAddStack.Push(_cobj);
                    break;
                case Token t when t.Type == TokenType.Format:
                    var cobj = makeChatObject(t.Value[0], parent);
                    cobj.Children = new ChatObject[] { ParseChat(cobj, root ?? (root = cobj)) };
                    if (cobj == root)
                    {
                        cobj.Children = cobj.Children.Concat(rootAddStack).ToArray();
                        rootAddStack.Clear();
                    }
                    return cobj;
                case Token t when t.Type == TokenType.String:
                    var c_obj = new ChatObject
                    {
                        Text = t.Value
                    };
                    c_obj.Children = new ChatObject[] { ParseChat(c_obj, root ?? (root = c_obj)) };
                    if (c_obj == root)
                    {
                        c_obj.Children = c_obj.Children.Concat(rootAddStack).ToArray();
                        rootAddStack.Clear();
                    }
                    return c_obj;
            }

            return null;
        }
    }
}