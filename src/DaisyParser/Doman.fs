namespace DaisyParser.Domain

open FParsec
module Common = 
  type Element = 
  | H1 
  | H2
  | H3 
  | H4 
  | H5  
  | H6
  | Span // span element for pages
  | Div // blocks (group) of text are also used for navigation by means of using the <div> element.

// http://dublincore.org/documents/1999/07/02/dces/
type DublinCoreLabelElement =
| Title 
| Creator
| Subject
| Description
| Publisher
| Contributor
| Date
| Type
| Format
| Identifier
| Source
| Language
| Relation
| Coverage
| Rights

type NCCMetaElement = 
| Charset
| Depth
| Files
| FootNotes
| Generator
| KByteSize
| MaxPageNormal
| MultimediaType
| Narrator
| PageFront
| PageNormal
| PageSpecial
| ProdNotes
| Producer 
| ProducerDate
| Revision
| RevisionDate
| SetInfo
| SideBars
| SourceData
| SourceEdition
| SourcePublisher
| SourceRights
| SourceTitle 
| TocItems
| TotalTime
| HttpEquiv

type NCCMetaItem = 
| DublinCoreLabelElement
| NCCMetaElement // additional elements to augment dublin standard 

module Daisy3 = 

  type DTB = { 
    PackageIdentifier : string 
    METADATA : string 
    Manifest : string
    Spine : string
    Tours : string
    Guide : string}

//http://www.daisy.org/z3986/specifications/daisy_202.html?q=publications/specifications/daisy_202.html#documents

module Daisy202 =
//The NCC document contains an index of navigable entry points into the DTB.
//"NCC.HTML" or "ncc.html".
// represents flow of narration similar but not necessarily the same as 
// serialised to XHTML 1.0
  type NCC = {
    Title : string 
    // biblogrpahic and technical description
    Meta : NCCMetaItem list }
    
