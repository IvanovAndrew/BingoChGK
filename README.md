# Bingo CHGK Telegram Bot 🤖🎲

A Telegram bot designed to help users train their **CHGK (What? Where? When?)** skills using a fun and engaging bingo format.

## 🎯 Features

- ⏰ Sends multiple daily prompts with **bingo-style CHGK realia** (e.g. “spoons”, “red flag”, “tattoos”).
- ❓ Asks real **CHGK-style questions** from a database related to the current realia.
- 🎓 Great for solo training or preparing for tournaments.
- 📅 Keeps track of what was asked and avoids repetition.

## 💡 How it works

1. The bot picks a thematic realia (concept or object).
2. Sends it to users several times a day as a challenge.
3. Upon request, sends one or more questions from a curated database related to that realia.
4. Users can reflect, guess the answer, then reveal the correct one.

## 📦 Technologies

- C# / .NET 8
- Telegram.Bot API
- Hosted on Azure (serverless)
- Custom parser and formatter for CHGK questions