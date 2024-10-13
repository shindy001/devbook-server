using Bogus;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;
using Microsoft.EntityFrameworkCore;

namespace DevBook.API.Features.BookStore;

internal sealed class BookStoreDataSeeder
{
	private readonly Random random = new();
	private const int MaxBookDataCound = 50;

	private readonly ProductCategory[] subcategories = [
			new ProductCategory
			{
				Name = "Sci-fi",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
			new ProductCategory
			{
				Name = "Romance",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
		];

	private readonly ProductCategory newReleases = new ProductCategory
	{
		Name = "New Releases",
		IsTopLevelCategory = false,
		Subcategories = [],
	};

	private readonly ProductCategory bestSellers = new ProductCategory
	{
		Name = "Bestsellers",
		IsTopLevelCategory = false,
		Subcategories = [],
	};

	private readonly ProductCategory preOrders = new ProductCategory
	{
		Name = "Pre-orders",
		IsTopLevelCategory = false,
		Subcategories = [],
	};

	public async Task<ProductCategory[]> SeedCategories(DevBookDbContext dbContext)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;

		List<ProductCategory> topLevelCategories = [
			new ProductCategory
			{
				Name = "Books",
				IsTopLevelCategory = true,
				Subcategories = subcategories.Select(x => x.Id).ToList(),
			}
		];

		await dbContext.AddRangeAsync(subcategories);
		await dbContext.AddRangeAsync(newReleases, bestSellers, preOrders);
		await dbContext.AddRangeAsync(topLevelCategories);

		return [.. subcategories, .. topLevelCategories];
	}

	public async Task SeedBooks(DevBookDbContext dbContext, ProductCategory[] productCategories)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;
		int bookFakerIndex = 0;

		var books = new Faker<Book>()
			.RuleFor(x => x.Name, () => GetBookData(bookFakerIndex).name)
			.RuleFor(x => x.ProductType, ProductType.Book)
			.RuleFor(x => x.RetailPrice, f => f.Random.Number(20, 35))
			.RuleFor(x => x.Price, f => f.Random.Number(5, 19))
			.RuleFor(x => x.DiscountAmmount, f => f.Random.Number(0, 1))

			.RuleFor(x => x.Author, () => GetBookData(bookFakerIndex).author)
			.RuleFor(x => x.Description, () => GetBookData(bookFakerIndex).description)
			.RuleFor(x => x.CoverImageUrl, () => GetBookData(bookFakerIndex).coverImageUrl)
			.RuleFor(x => x.ProductCategoryIds, f => productCategories
				.Skip(f.Random.Number(0, productCategories.Length - 1))
				.Take(f.Random.Number(0, productCategories.Length - 1))
				.Select(x => x.Id)
				.ToList())
			.FinishWith((f, x) =>
			{
				var additionalCategory = GetAdditionalProductCategory(bookFakerIndex);
				if (additionalCategory != null)
				{
					x.ProductCategoryIds.Add(additionalCategory.Id);
				}
				bookFakerIndex += 1;
			})
			.Generate(MaxBookDataCound);

		await dbContext.AddRangeAsync(books);
	}

	private ProductCategory? GetAdditionalProductCategory(int bookIndex)
	{
		if (bookIndex <= 0 || bookIndex <= 11)
		{
			return newReleases;
		}
		else if (bookIndex <= 23)
		{
			return bestSellers;
		}
		else if (bookIndex <= 35)
		{
			return preOrders;
		}
		else
		{
			return null;
		}
	}

	private (string name, string author, string coverImageUrl, string description) GetBookData(int bookIndex)
	{
		if (bookIndex < 0 || bookIndex > BookData.Length)
		{
			throw new IndexOutOfRangeException($"Invalid bookIndex value '{bookIndex}', only indexes between 0 and {BookData.Length} are allowed.");
		}

		return BookData[bookIndex];
	}

	private readonly (string name, string author, string coverImageUrl, string description)[] BookData = [
		new ("Don Quixote", "Miguel de Cervantes", "https://books.google.com/books/content?id=3qxaBAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A novel that follows the adventures of a delusional nobleman who believes he is a knight. Along with his loyal squire, Sancho Panza, he embarks on a series of absurd yet heroic exploits."),
		new ("Alice's Adventures in Wonderland", "Lewis Carroll", "https://books.google.com/books/content?id=9I5yDwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Alice falls through a rabbit hole into a fantasy world where she encounters bizarre creatures and whimsical adventures. A classic of children's literature."),
		new ("The Adventures of Huckleberry Finn", "Mark Twain", "https://books.google.com/books/content?id=ClcOEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Huckleberry Finn travels down the Mississippi River with a runaway slave, Jim. The novel explores themes of freedom, race, and morality."),
		new ("The Adventures of Tom Sawyer", "Mark Twain", "https://books.google.com/books/content?id=n3wx_HaarbQC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Tom Sawyer, a mischievous boy, embarks on adventures with his friends, including witnessing a murder and searching for hidden treasure."),
		new ("Treasure Island", "Robert Louis Stevenson", "https://books.google.com/books/content?id=0I68EAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A young boy named Jim Hawkins discovers a treasure map and embarks on a dangerous journey to find the treasure. Filled with pirates, mutiny, and adventure."),
		new ("Pride and Prejudice", "Jane Austen", "https://books.google.com/books/content?id=tvUlKY9E3Q0C&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Elizabeth Bennet navigates love, society, and family in 19th-century England. The novel explores issues of class, marriage, and morality."),
		new ("Wuthering Heights", "Emily Brontë", "https://books.google.com/books/content?id=S2Yvf-tWNdUC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A dark and passionate tale of love and revenge set on the Yorkshire moors. The relationship between Heathcliff and Catherine Earnshaw is central to the novel's tragedy."),
		new ("Jane Eyre", "Charlotte Brontë", "https://books.google.com/books/content?id=2YllEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An orphaned girl grows up to become a governess and falls in love with her employer, Mr. Rochester. The novel explores themes of independence, morality, and love."),
		new ("Moby Dick", "Herman Melville", "https://books.google.com/books/content?id=XV8XAAAAYAAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Captain Ahab is obsessed with hunting the white whale, Moby Dick, at all costs. The novel explores themes of fate, obsession, and the conflict between man and nature."),
		new ("The Scarlet Letter", "Nathaniel Hawthorne", "https://books.google.com/books/content?id=Iqtb9zytWCIC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A woman in Puritan New England is shunned for bearing an illegitimate child. The novel explores guilt, sin, and redemption."),
		new ("Gulliver's Travels", "Jonathan Swift", "https://books.google.com/books/content?id=lqUVDAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Lemuel Gulliver embarks on a series of fantastical voyages to strange lands, including Lilliput and Brobdingnag. A satire of human nature and society."),
		new ("The Pilgrim's Progress", "John Bunyan", "https://books.google.com/books/content?id=ZS6GAAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An allegory of a Christian's journey through life, following the protagonist, Christian, as he navigates spiritual trials and challenges."),
		new ("A Christmas Carol", "Charles Dickens", "https://books.google.com/books/content?id=LelLAAAAcAAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Ebenezer Scrooge, a miserly old man, is visited by three ghosts on Christmas Eve, leading him to a transformation in his outlook on life and generosity."),
		new ("David Copperfield", "Charles Dickens", "https://books.google.com/books/content?id=b81xCAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The semi-autobiographical story of David Copperfield's journey from an impoverished boy to a successful author. It explores themes of hardship, resilience, and personal growth."),
		new ("A Tale of Two Cities", "Charles Dickens", "https://books.google.com/books/content?id=i6r6Oyf7RpAC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Set in London and Paris during the French Revolution, the novel follows the lives of several characters, including Charles Darnay and Sydney Carton, amidst political turmoil."),
		new ("Little Women", "Louisa May Alcott", "https://books.google.com/books/content?id=WoNwISIwvmwC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The story of four sisters—Meg, Jo, Beth, and Amy—growing up during the American Civil War. The novel explores family, love, and personal aspirations."),
		new ("Great Expectations", "Charles Dickens", "https://books.google.com/books/content?id=opcZAwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Pip, an orphan, aspires to improve his social standing after receiving a mysterious fortune. The novel explores themes of ambition, social class, and personal growth."),
		new ("The Hobbit, or, There and Back Again", "J.R.R. Tolkien", "https://books.google.com/books/content?id=Ts_gmPmzxtUC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Bilbo Baggins, a reluctant hobbit, embarks on a dangerous adventure with a group of dwarves to reclaim a stolen treasure from a dragon."),
		new ("Frankenstein, or, the Modern Prometheus", "Mary Shelley", "https://books.google.com/books/content?id=cRrGEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Victor Frankenstein creates a living being from dead body parts, only to be horrified by his creation. The novel explores themes of ambition, guilt, and the consequences of scientific experimentation."),
		new ("Oliver Twist", "Charles Dickens", "https://books.google.com/books/content?id=yPJZEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An orphan boy named Oliver escapes a life of poverty and crime in Victorian England. The novel highlights the harsh conditions faced by the poor."),
		new ("Uncle Tom's Cabin", "Harriet Beecher Stowe", "https://books.google.com/books/content?id=EAYR49A9YwwC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A powerful anti-slavery novel that depicts the harsh realities of slavery in the American South. It helped fuel the abolitionist movement."),
		new ("Crime and Punishment", "Fyodor Dostoyevsky", "https://books.google.com/books/content?id=nVGKDgAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Raskolnikov, a poor student, commits a murder and struggles with guilt and morality in this psychological exploration of crime, punishment, and redemption."),
		new ("Madame Bovary", "Gustave Flaubert", "https://books.google.com/books/content?id=1kk5kpXB7VwC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Emma Bovary, dissatisfied with her provincial life and marriage, seeks excitement in adulterous affairs, leading to her downfall."),
		new ("The Return of the King", "J.R.R. Tolkien", "https://books.google.com/books/content?id=KUq8NhaYpxkC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The final installment of 'The Lord of the Rings' trilogy. It chronicles the defeat of Sauron and the restoration of Aragorn to the throne."),
		new ("Dracula", "Bram Stoker", "https://books.google.com/books/content?id=6sbiEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The story of Count Dracula's attempt to move from Transylvania to England in search of new blood, and the group of people determined to stop him."),
		new ("The Three Musketeers", "Alexandre Dumas", "https://books.google.com/books/content?id=waiGEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "D'Artagnan joins the Three Musketeers—Athos, Porthos, and Aramis—in their adventures as they protect the king and fight intrigue in 17th-century France."),
		new ("Brave New World", "Aldous Huxley", "https://books.google.com/books/content?id=QIJ5EAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A dystopian novel set in a future society that sacrifices individuality for the sake of order and stability. It explores the consequences of state control and technological advancement."),
		new ("War and Peace", "Leo Tolstoy", "https://books.google.com/books/content?id=jVMEAAAAYAAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An epic tale set against the backdrop of the Napoleonic wars, following the lives of several aristocratic families in Russia."),
		new ("To Kill a Mockingbird", "Harper Lee", "https://books.google.com/books/content?id=c9pbL5epUN4C&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A young girl, Scout, grows up in the racially charged American South, where her father defends a black man wrongly accused of raping a white woman."),
		new ("The Wizard of Oz", "L. Frank Baum", "https://books.google.com/books/content?id=hRd7dJ-9G1IC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Dorothy and her dog Toto are swept away to the magical land of Oz, where they embark on a journey to meet the Wizard and return home."),
		new ("Les Misérables", "Victor Hugo", "https://books.google.com/books/content?id=h6A6DAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The novel follows the lives of several characters, including ex-convict Jean Valjean, in the aftermath of the French Revolution, exploring themes of justice, love, and redemption."),
		new ("The Secret Garden", "Frances Hodgson Burnett", "https://books.google.com/books/content?id=sVtSDwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A young orphan, Mary Lennox, discovers a hidden, neglected garden and works to restore it. The novel emphasizes the healing power of nature and friendship."),
		new ("Animal Farm", "George Orwell", "https://books.google.com/books/content?id=nkalO3OsoeMC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A political allegory where farm animals overthrow their human owner to establish a society, only for it to become corrupt under the leadership of the pigs."),
		new ("The Great Gatsby", "F. Scott Fitzgerald", "https://books.google.com/books/content?id=iWA-DwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A novel about the mysterious millionaire Jay Gatsby and his obsession with Daisy Buchanan. It explores themes of wealth, love, and the American Dream."),
		new ("The Little Prince", "Antoine de Saint-Exupéry", "https://books.google.com/books/content?id=CQYg20lTHtMC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A young prince travels from planet to planet, learning life lessons about love, loss, and human nature."),
		new ("The Call of the Wild", "Jack London", "https://books.google.com/books/content?id=v1WMCgAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A domesticated dog named Buck is stolen from his home and thrust into the wilds of Alaska during the Klondike Gold Rush, where he must learn to survive."),
		new ("20,000 Leagues Under the Sea", "Jules Verne", "https://books.google.com/books/content?id=5t9BBAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Captain Nemo takes his guests on a journey aboard the submarine Nautilus, exploring the wonders of the ocean while hiding from society."),
		new ("Anna Karenina", "Leo Tolstoy", "https://books.google.com/books/content?id=1DooDwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A tragic love story about a Russian aristocrat, Anna, who has an affair that leads to her downfall. It explores themes of love, family, and society."),
		new ("The Wind in the Willows", "Kenneth Grahame", "https://books.google.com/books/content?id=bqhaAAAAMAAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A tale of friendship and adventure among animals living along the riverbank, featuring characters like Mole, Rat, Toad, and Badger."),
		new ("The Picture of Dorian Gray", "Oscar Wilde", "https://books.google.com/books/content?id=Us0gTQUSAvsC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Dorian Gray remains eternally youthful while his portrait ages, reflecting the consequences of his immoral lifestyle."),
		new ("The Grapes of Wrath", "John Steinbeck", "https://books.google.com/books/content?id=ClXiwSYzjtYC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "The Joad family, displaced by the Dust Bowl, travels to California in search of a better life, only to face further hardship and injustice."),
		new ("Sense and Sensibility", "Jane Austen", "https://books.google.com/books/content?id=bcUNAAAAQAAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Two sisters, Elinor and Marianne, navigate love and heartbreak in 19th-century England. The novel explores the balance between emotion and reason."),
		new ("The Last of the Mohicans", "James Fenimore Cooper", "https://books.google.com/books/content?id=Xnpvf7RBVNgC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Set during the French and Indian War, this novel follows the adventures of Hawkeye and his Mohican friends as they navigate the dangers of frontier life."),
		new ("Tess of the d'Urbervilles", "Thomas Hardy", "https://books.google.com/books/content?id=mi2xNBZDZ7sC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A tragic story of a young woman, Tess, who is betrayed by the men in her life and society, leading to her downfall."),
		new ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "https://books.google.com/books/content?id=6gciEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An orphaned boy, Harry Potter, discovers he is a wizard and attends Hogwarts School of Witchcraft and Wizardry. The first in a beloved fantasy series."),
		new ("Heidi", "Johanna Spyri", "https://books.google.com/books/content?id=aXLOEAAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A young girl named Heidi is sent to live with her grandfather in the Swiss Alps, where she grows to love the mountains and brings joy to those around her."),
		new ("Ulysses", "James Joyce", "https://books.google.com/books/content?id=rTD0pZvwHD8C&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A modern retelling of Homer's Odyssey, set in a single day in Dublin, as Leopold Bloom navigates various episodes that mirror the ancient epic."),
		new ("The Complete Sherlock Holmes", "Arthur Conan Doyle", "https://books.google.com/books/content?id=1zSnAwAAQBAJ&printsec=frontcover&img=1&zoom=2&source=gbs_api", "A collection of stories featuring Sherlock Holmes, the iconic detective, and his companion Dr. Watson as they solve various mysteries."),
		new ("The Count of Monte Cristo", "Alexandre Dumas", "https://books.google.com/books/content?id=_-6BzIqpLV0C&printsec=frontcover&img=1&zoom=2&source=gbs_api", "Edmond Dantès is wrongfully imprisoned and later escapes, seeking revenge on those who betrayed him. A tale of justice, revenge, and redemption."),
		new ("The Old Man and the Sea", "Ernest Hemingway", "https://books.google.com/books/content?id=K1cIZmFe7KoC&printsec=frontcover&img=1&zoom=2&source=gbs_api", "An aging fisherman, Santiago, engages in an epic struggle to catch a giant marlin, reflecting themes of perseverance and man's battle against nature.")
	];
}
