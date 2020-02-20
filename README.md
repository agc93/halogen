# Halogen

> A video metadata management framework
> This is ***highly*** work-in-progress! Not recommended for use at this time.

## Introduction

Halogen is intended as a framework and/or building block for tagging, categorising and organising video libraries. Unlike similar libraries though, Halogen is **heavily** opinionated and aims to provide a complete metadata management layer to simplify video library management.

### Context

Some time ago, I hacked together a library that used a combination of a local data file and embedded tags in video files to manage my library of drone recordings. It was buggy, slow and frankly a UX nightmare. Halogen is an attempt to strip out the metadata layer into something a bit more robust and extensible without the technical overhead of a userspace app.

### Uses

Once it's complete, it should be possible to build highly specialised video library tools with a minimum of ceremony, assuming the metadata requirements are *reasonably* light. Applications will only need to define (internally, Halogen doesn't care) important metadata requirements with either key-value pairs (recommended) or a lightweight object model and Halogen will embed that data directly into the video file and track the file on disk based on that metadata.

> Currently, Halogen embeds all metadata directly into the file. While this is fine for very small data requirements (i.e. `drone=mavicpro`), it's probably going to run into problems as your metadata requirements grow. As such, parts of Halogen have been retooled to support (in future) storing application data/metadata separately and connecting it to video files with in-file tags.
