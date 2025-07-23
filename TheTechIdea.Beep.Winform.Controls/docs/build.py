#!/usr/bin/env python3
"""
Build script for Beep Controls documentation.
This script provides a convenient way to build and serve the documentation locally.
"""

import argparse
import os
import subprocess
import sys
import webbrowser
from http.server import HTTPServer, SimpleHTTPRequestHandler
from pathlib import Path
import threading
import time

def run_command(cmd, cwd=None):
    """Run a shell command and return the result."""
    try:
        result = subprocess.run(cmd, shell=True, cwd=cwd, capture_output=True, text=True)
        if result.returncode != 0:
            print(f"Error running command: {cmd}")
            print(f"Error output: {result.stderr}")
            return False
        return True
    except Exception as e:
        print(f"Exception running command: {cmd}")
        print(f"Exception: {e}")
        return False

def install_requirements():
    """Install required Python packages."""
    print("Installing required packages...")
    return run_command("pip install -r requirements.txt", cwd="docs")

def build_docs(builder="html"):
    """Build the documentation."""
    print(f"Building documentation with {builder} builder...")
    
    # Change to docs directory
    docs_dir = Path("docs")
    if not docs_dir.exists():
        print("Error: docs directory not found!")
        return False
    
    # Run sphinx-build
    cmd = f"sphinx-build -b {builder} source build/{builder}"
    return run_command(cmd, cwd=docs_dir)

def clean_build():
    """Clean the build directory."""
    print("Cleaning build directory...")
    
    docs_dir = Path("docs")
    build_dir = docs_dir / "build"
    
    if build_dir.exists():
        import shutil
        shutil.rmtree(build_dir)
        print("Build directory cleaned.")
    else:
        print("Build directory doesn't exist, nothing to clean.")

def serve_docs(port=8000):
    """Serve the documentation locally."""
    docs_dir = Path("docs/build/html")
    
    if not docs_dir.exists():
        print("No built documentation found. Building first...")
        if not build_docs():
            print("Failed to build documentation.")
            return False
    
    print(f"Serving documentation at http://localhost:{port}")
    print("Press Ctrl+C to stop the server")
    
    os.chdir(docs_dir)
    
    # Start server in a separate thread
    def start_server():
        httpd = HTTPServer(('localhost', port), SimpleHTTPRequestHandler)
        httpd.serve_forever()
    
    server_thread = threading.Thread(target=start_server, daemon=True)
    server_thread.start()
    
    # Open browser after a short delay
    time.sleep(1)
    webbrowser.open(f"http://localhost:{port}")
    
    try:
        # Keep the main thread alive
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("\nStopping server...")
        return True

def watch_and_rebuild():
    """Watch for file changes and rebuild automatically."""
    try:
        import watchdog
        from watchdog.observers import Observer
        from watchdog.events import FileSystemEventHandler
    except ImportError:
        print("watchdog package not installed. Install with: pip install watchdog")
        return False
    
    class DocHandler(FileSystemEventHandler):
        def __init__(self):
            self.last_build = 0
        
        def on_modified(self, event):
            if not event.is_directory and event.src_path.endswith(('.rst', '.md', '.py')):
                current_time = time.time()
                if current_time - self.last_build > 2:  # Debounce rebuilds
                    print(f"File changed: {event.src_path}")
                    print("Rebuilding documentation...")
                    if build_docs():
                        print("Rebuild successful!")
                    else:
                        print("Rebuild failed!")
                    self.last_build = current_time
    
    # Initial build
    if not build_docs():
        print("Initial build failed!")
        return False
    
    # Set up file watcher
    event_handler = DocHandler()
    observer = Observer()
    observer.schedule(event_handler, "docs/source", recursive=True)
    observer.start()
    
    print("Watching for file changes... Press Ctrl+C to stop")
    
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        observer.stop()
        print("\nStopping file watcher...")
    
    observer.join()
    return True

def main():
    parser = argparse.ArgumentParser(description="Build Beep Controls documentation")
    parser.add_argument("command", choices=["install", "build", "serve", "clean", "watch", "pdf"], 
                       help="Command to execute")
    parser.add_argument("--port", type=int, default=8000, help="Port for serve command")
    
    args = parser.parse_args()
    
    if args.command == "install":
        success = install_requirements()
    elif args.command == "build":
        success = build_docs()
    elif args.command == "serve":
        success = serve_docs(args.port)
    elif args.command == "clean":
        clean_build()
        success = True
    elif args.command == "watch":
        success = watch_and_rebuild()
    elif args.command == "pdf":
        success = build_docs("latex")
        if success:
            print("Running LaTeX to generate PDF...")
            success = run_command("make", cwd="docs/build/latex")
    else:
        print(f"Unknown command: {args.command}")
        success = False
    
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()