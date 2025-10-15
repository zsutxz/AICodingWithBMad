"""
Minimal 'Hello World' coding agent example
"""

import asyncio
import os
from claude_agent_sdk import query, ClaudeAgentOptions

async def main():
    # Check if API key is set
    if not os.environ.get("ANTHROPIC_API_KEY"):
        print("Error: ANTHROPIC_API_KEY environment variable is not set.")
        print("Please set your API key in the .env file or as an environment variable.")
        print("You can get your API key from https://console.anthropic.com/")
        return
    
    # Create options with system prompt
    options = ClaudeAgentOptions(
        system_prompt="You are a helpful coding agent that provides clear, accurate code examples and explanations."
    )
    
    # Send a hello message to the agent and stream responses
    try:
        async for message in query(
            prompt="Hello, I'd like to learn about Python coding. Can you show me a simple 'Hello World' example?",
            options=options
        ):
            print(f"Agent response: {message}")
    except Exception as e:
        print(f"Error communicating with Claude API: {e}")
        print("Please check your API key and internet connection.")

if __name__ == "__main__":
    asyncio.run(main())